#!/bin/bash
# Date: November 2022
# Author: Michael Schletz
# Description: Installiert die .NET SDK

VERSION=$(curl https://dotnetcli.azureedge.net/dotnet/Sdk/6.0/latest.version)
INSTALLFILE=dotnet-sdk-$VERSION-linux-x64.tar.gz
DOTNET_HOME=/usr/bin
echo Lade .NET $VERSION. Bitte warten...
curl https://dotnetcli.azureedge.net/dotnet/Sdk/$VERSION/$INSTALLFILE > /tmp/$INSTALLFILE
echo Entpacke .NET
tar zxf /tmp/$INSTALLFILE -C $DOTNET_HOME
rm /tmp/$INSTALLFILE
