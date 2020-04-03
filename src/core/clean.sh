#!/bin/bash

sudo find ../../../WebExpress/ -name "obj" -exec rm -Rf {} \;
sudo find . -name "obj" -exec rm -Rf {} \;

export PATH=$PATH:/usr/share/dotnet-sdk/
export DOTNET_ROOT=/usr/share/dotnet-sdk/ 

dotnet clean
