#!/usr/bin/env bash

set -e

TAG="6.3.0"

SOURCE="https://github.com/DuendeSoftware/IdentityServer.Quickstart.UI/archive/refs/tags/$TAG.zip"
curl -L -o ui.zip "$SOURCE"

unzip -d ui ui.zip

[[ -d Pages ]] || mkdir Pages
[[ -d wwwroot ]] || mkdir wwwroot

cp -r ./ui/IdentityServer.Quickstart.UI-$TAG/Pages/* Pages
cp -r ./ui/IdentityServer.Quickstart.UI-$TAG/wwwroot/* wwwroot

rm -rf ui ui.zip

# Workaround until https://github.com/DuendeSoftware/IdentityServer.Quickstart.UI/issues/14 is fixed
find Pages -name "*.cs" -type f -exec sed -i 's/using IdentityModel;/using Duende.IdentityModel;/g' {} +
