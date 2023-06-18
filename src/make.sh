#!/bin/bash
# Erstellt WebExpress

export PATH=$PATH:/usr/share/dotnet-sdk/
export DOTNET_ROOT=/usr/share/dotnet-sdk/ 

dotnet build --configuration Release
#dotnet publish
