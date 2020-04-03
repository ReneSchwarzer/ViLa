#!/bin/bash

export PATH=$PATH:/usr/share/dotnet-sdk/
export DOTNET_ROOT=/usr/share/dotnet-sdk/ 
export DOTNET_CLI_TELEMETRY_OPTOUT=1

dir=´pwd´

cd /opt/mtwb
./MtWb.App

cd $dir