#!/bin/bash
# Erstellt und verteilt den ViLa-Service, ohne ihn zu starten.
# Das Starten und Stoppen erfolgt automatisch (Starten beim booten) 
# oder manuell über die Skripte start.sh und stop.sh.

export PATH=$PATH:/usr/share/dotnet-sdk/
export DOTNET_ROOT=/usr/share/dotnet-sdk/ 

dotnet build
dotnet publish

if (sudo systemctl -q is-enabled vila.service)
then
	sudo systemctl disable vila.service 
fi

sudo mkdir -p /opt/vila
sudo chmod +x /opt/vila


##cp -Rf ViLa/bin/Debug/netcoreapp3.1/publish/* /opt/vila
cp -Rf ViLa.App/bin/Debug/netcoreapp3.1/publish/* /opt/vila
cp vila.sh /opt/vila
sudo chmod +x /opt/vila/vila.sh

sudo cp vila.service /etc/systemd/system
sudo systemctl enable vila.service
