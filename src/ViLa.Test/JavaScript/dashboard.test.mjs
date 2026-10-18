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
    const options = [];
    let timerId = 0;
    let disconnected = false;
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
        document: { hidden, documentElement: {}, getElementById: () => chartElement },
        getComputedStyle: () => ({ color: "rgb(238, 238, 238)", getPropertyValue: () => "#444" }),
        MutationObserver: class {
            /** Starts the stub observer. */
            observe() {}
            /** Records observer cleanup. */
            disconnect() { disconnected = true; }
        },
        AbortController,
        fetch: fetchHandler,
        setTimeout: (callback, delay) => { timers.set(++timerId, { callback, delay }); return timerId; },
        clearTimeout: id => timers.delete(id),
        webexpress: { webui: {
            Ctrl: class {
                /** Releases the stub base controller. */
                destroy() {}
            },
            Controller: {
                registerClass: () => {},
                getInstanceByElement: () => ({ updateData: data => updates.push(data), options: value => options.push(value) })
            }
        } }
    });
    vm.runInContext(source, context);
    const controller = new context.vila.ChartCtrl(host);
    return { controller, host, metrics, timers, classes, updates, options, disconnected: () => disconnected };
}

/**
 * Flushes completed fetch continuations without advancing polling timers.
 * @returns {Promise<void>} Resolves after queued promise work finishes.
 */
function settle() {
    return new Promise(resolve => setImmediate(resolve));
}

const snapshot = { ActiveCharging: true, CurrentPower: "6,00", Power: "1,00", Cost: "0,28", MeasurementTime: "10m", ChartLabels: ["0"], ChartData: ["6"] };

test("updates metrics as text and sends numeric values to the WebExpress chart", async () => {
    const harness = createHarness(async () => ({ ok: true, json: async () => snapshot }));
    await settle();
    assert.equal(harness.metrics.get("#vila-current").textContent, "6,00 kW");
    assert.equal(harness.metrics.get("#vila-connection").textContent, "Live");
    assert.equal(harness.updates[0].datasets[0].data[0], 6);
    assert.equal(harness.options[0].scales.y.ticks.color, "rgb(238, 238, 238)");
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

test("does not poll in a hidden tab and cleans up its timer and observer", () => {
    let requests = 0;
    const harness = createHarness(() => { requests++; }, true);
    assert.equal(requests, 0);
    assert.equal(harness.timers.size, 1);
    harness.controller.destroy();
    assert.equal(harness.timers.size, 0);
    assert.equal(harness.disconnected(), true);
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
