#!/bin/bash
# Erstellt und verteilt den MtWb-Service, ohne ihn zu starten.
# Das Starten und Stoppen erfolgt automatisch (Starten beim booten) 
# oder manuell über die Skripte start.sh und stop.sh.

export PATH=$PATH:/usr/share/dotnet-sdk/
export DOTNET_ROOT=/usr/share/dotnet-sdk/ 

dotnet build
dotnet publish

if (sudo systemctl -q is-enabled mtwb.service)
then
	sudo systemctl disable mtwb.service 
fi

sudo mkdir -p /opt/mtwb
sudo chmod +x /opt/mtwb


cp -Rf MtWb/bin/Debug/netcoreapp3.0/publish/* /opt/mtwb
cp mtwb.sh /opt/mtwb
sudo chmod +x /opt/mtwb/mtwb.sh

sudo cp mtwb.service /etc/systemd/system
sudo systemctl enable mtwb.service
