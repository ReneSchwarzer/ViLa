import test from "node:test";
import assert from "node:assert/strict";
import { readFileSync } from "node:fs";
import vm from "node:vm";

const source = readFileSync(new URL("../../ViLa/Assets/js/vila.dashboard.js", import.meta.url), "utf8");

/**
 * Creates a dashboard controller with isolated browser services and manually controlled timers.
 * @param {Function} fetchHandler - The fetch implementation used by the test.
 * @param {boolean} [hidden=false] - Whether the document starts in the background.
 * @returns {Object} The controller, metrics, timers, and observed chart operations.
 */
function createHarness(fetchHandler, hidden = false) {
    const metrics = new Map();
    const timers = new Map();
    const classes = new Set();
    const updates = [];
    let timerId = 0;
    const chartElement = {};
    const host = {
        isConnected: true,
        dataset: { url: "/vila/api/1/charging", currency: "€", active: "Charging", idle: "Ready", connected: "Live", offline: "Offline" },
        classList: {
            add: value => classes.add(value),
            remove: value => classes.delete(value),
            toggle: (value, enabled) => enabled ? classes.add(value) : classes.delete(value)
        },
        querySelector: selector => {
            if (selector === "#vila-power-chart") {
                return chartElement;
            }
            if (!metrics.has(selector)) {
                metrics.set(selector, { textContent: "previous" });
            }
            return metrics.get(selector);
        }
    };
    const context = vm.createContext({
        document: { hidden },
        AbortController,
        fetch: fetchHandler,
        setTimeout: (callback, delay) => { timers.set(++timerId, { callback, delay }); return timerId; },
        clearTimeout: id => timers.delete(id),
        webexpress: { webapp: { KanbanCtrl: class {} }, webui: {
            Ctrl: class {
                /** Releases the stub base controller. */
                destroy() {}
            },
            Controller: {
                registerClass: () => {},
                getInstanceByElement: () => ({ updateData: data => updates.push(data) })
            }
        } }
    });
    vm.runInContext(source, context);
    const controller = new context.vila.ChartCtrl(host);
    return { controller, host, metrics, timers, classes, updates };
}

/**
 * Flushes completed fetch continuations without advancing polling timers.
 * @returns {Promise<void>} Resolves after queued promise work finishes.
 */
function settle() {
    return new Promise(resolve => setImmediate(resolve));
}

const snapshot = { ActiveCharging: true, CurrentPower: "6,00", Power: "1,00", Cost: "0,28", MeasurementTime: "10m", ChartLabels: ["0"], ChartData: ["6"] };

test("refreshes the vehicle catalog for local and document-level save events and removes its listener", () => {
    // arrange
    const listeners = new Map();
    const createDialog = {};
    const editDialog = {};
    const deleteDialog = {};
    let loads = 0;
    const table = { load: () => loads++ };
    const host = {
        querySelector: selector => ({
            "#vehicle-create": createDialog,
            "#vehicle-edit": editDialog,
            "#vehicle-delete": deleteDialog,
            "#vehicle_catalog": table,
            "#vehicle-search": {}
        })[selector]
    };
    const document = {
        addEventListener: (name, listener, capture) => listeners.set(name, { listener, capture }),
        removeEventListener: (name, listener, capture) => {
            assert.equal(listeners.get(name).listener, listener);
            assert.equal(listeners.get(name).capture, capture);
            listeners.delete(name);
        }
    };
    const context = vm.createContext({
        document,
        window: { location: { href: "http://localhost/vila/settings/vehicles" } },
        URL,
        webexpress: { webapp: { KanbanCtrl: class {} }, webui: {
            Ctrl: class {
                /** Releases the stub base controller. */
                destroy() {}
            },
            Event: { SAVED_EVENT: "saved" },
            Controller: { registerClass: () => {}, getInstanceByElement: element => element }
        } }
    });
    vm.runInContext(source, context);
    const controller = new context.vila.VehicleCatalogCtrl(host);
    const listener = listeners.get("saved").listener;

    // act
    listener({ target: { closest: () => createDialog } });
    listener({ target: document, detail: { sender: { closest: () => editDialog } } });
    listener({ target: document, detail: { sender: { closest: () => ({}) } } });

    listener({ target: document, detail: { sender: { closest: () => deleteDialog } } });

    // validation
    assert.equal(loads, 3);
    controller.destroy();
    assert.equal(listeners.size, 0);
});

test("updates metrics as text and sends numeric values to the WebExpress chart", async () => {
    const harness = createHarness(async () => ({ ok: true, json: async () => snapshot }));
    await settle();
    assert.equal(harness.metrics.get("#vila-current").textContent, "6,00 kW");
    assert.equal(harness.metrics.get("#vila-connection").textContent, "Live");
    assert.equal(harness.updates[0].datasets[0].data[0], 6);
    assert.equal(harness.updates.length, 1);
    harness.controller.destroy();
});

test("preserves values on failure and recovers on the next successful request", async () => {
    let success = false;
    const harness = createHarness(async () => ({ ok: success, json: async () => snapshot }));
    await settle();
    assert.equal(harness.metrics.get("#vila-connection").textContent, "Offline");
    assert.equal(harness.updates.length, 0);
    success = true;
    const [id, timer] = [...harness.timers].find(([, value]) => value.delay === 5000);
    harness.timers.delete(id);
    await timer.callback();
    assert.equal(harness.metrics.get("#vila-connection").textContent, "Live");
    assert.equal(harness.classes.has("vila-offline"), false);
    harness.controller.destroy();
});

test("does not poll in a hidden tab and cleans up its timer", () => {
    let requests = 0;
    const harness = createHarness(() => { requests++; }, true);
    assert.equal(requests, 0);
    assert.equal(harness.timers.size, 1);
    harness.controller.destroy();
    assert.equal(harness.timers.size, 0);
});

test("aborts an outstanding request without scheduling another poll after destruction", async () => {
    let signal;
    const harness = createHarness((url, options) => {
        signal = options.signal;
        return new Promise((resolve, reject) => signal.addEventListener("abort", () => reject(new Error("aborted"))));
    });
    assert.equal([...harness.timers.values()].some(timer => timer.delay === 5000), false);
    harness.controller.destroy();
    await settle();
    assert.equal(signal.aborted, true);
    assert.equal(harness.timers.size, 0);
    assert.equal(harness.updates.length, 0);
});

test("rejects malformed snapshots without replacing existing chart values", async () => {
    const harness = createHarness(async () => ({ ok: true, json: async () => ({ ChartData: null }) }));
    await settle();
    assert.equal(harness.updates.length, 0);
    assert.equal(harness.metrics.get("#vila-connection").textContent, "Offline");
    harness.controller.destroy();
});

test("shows the live section only during an active charging session", async () => {
    let active = false;
    const harness = createHarness(async () => ({
        ok: true,
        json: async () => ({ ...snapshot, ActiveCharging: active })
    }));
    await settle();
    assert.equal(harness.classes.has("vila-charging"), false);
    for (const charging of [true, false]) {
        active = charging;
        const [id, timer] = [...harness.timers].find(([, value]) => value.delay === 5000);
        harness.timers.delete(id);
        await timer.callback();
        assert.equal(harness.classes.has("vila-charging"), charging);
    }
    assert.equal(harness.updates.length, 1);
    harness.controller.destroy();
});

/**
 * Verifies that assignment moves retain their source across asynchronous persistence.
 */
test("data kanban sends the expected source without submitting a form", () => {
    // arrange
    const sent = [];
    const listeners = new Map();
    const host = {
        addEventListener: (name, listener) => listeners.set(name, listener),
        removeEventListener: name => listeners.delete(name)
    };
    const context = vm.createContext({ webexpress: {
        webui: { Ctrl: class {}, Event: { MOVE_EVENT: "move" }, Controller: { registerClass: () => {} } },
        webapp: { KanbanCtrl: class {
            /** Initializes the persistence stub. */
            _initRestPersistence() {}
            /**
             * Records the request supplied to standard DataKanban persistence.
             * @param {Object} payload - The move request.
             */
            _sendStateToServer(payload) { sent.push(payload); }
            /** Releases the persistence stub. */
            destroy() {}
        } }
    } });
    vm.runInContext(source, context);
    const controller = new context.vila.AssignmentBoardCtrl();
    controller._element = host;
    controller._initRestPersistence(host);

    // act
    listeners.get("move")({ detail: { oldColumnId: "none" } });
    controller._sendStateToServer({ cardId: "first", columnId: "car-a" });
    listeners.get("move")({ detail: { oldColumnId: "car-a" } });
    controller._sendStateToServer({ cardId: "first", columnId: "car-b" });
    controller.destroy();

    // validation
    assert.equal(sent[0].source, "none");
    assert.equal(sent[0].columnId, "car-a");
    assert.equal(sent[1].source, "car-a");
    assert.equal(sent[1].columnId, "car-b");
    assert.equal(listeners.size, 0);
});
