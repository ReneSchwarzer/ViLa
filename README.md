# Visuelles Laden (ViLa)
IoT Projekt zur Steuerung und Kontrolle einer Wallbox.

# Beschreibung
Die Software steuert eine Wallbox (z.B. Heidelberg Wallbox Home Eco) und macht diese Smart. Dabei werden die Fähigkeiten der Wallbox um folgende Funktionen erweitert:

- Mandantenfähig (noch nicht umgesetzt)
- Entsperrung der Wallbox nur mit Zugangskontrolle (teilweise umgesetzt)
- Zählen der Verbräuche 
- Abrechnung je nach Mandanten (noch nicht umgesetzt)
- Anzeige der aktuellen Leistungsaufnahme/ Ampere (teilweise umgesetzt)
- Als Webseite (nur im WLAN sichtbar)

# Plattform
ViLa ist eine WebApp, welche in C# speziell für den Raspberry Pi entwickelt wurde.

# Voraussetzungen
- Raspberry Pi 3B
- Raspbian Buster Lite (https://downloads.raspberrypi.org/raspbian_lite_latest)
- .NET Core 5.0 SDK - (https://dotnet.microsoft.com/download/dotnet-core)
- WebExpress (https://github.com/ReneSchwarzer/WebExpress.git)
- WebExpressAgent (https://github.com/ReneSchwarzer/WebExpressAgent.git)

# Einrichtung 
siehe https://github.com/ReneSchwarzer/ViLa/blob/master/doc/ViLa.docx

# Verwendete Bibliotheken
- https://getbootstrap.com/
- https://www.chartjs.org
- https://jquery.com/
- https://summernote.org/
- https://popper.js.org/

# Stichwörter
#Wallbox #S0 #Raspberry #Raspbian #IoT #NETCore #WebExpress #DIN43864 #Heidelberg #Home #Eco 