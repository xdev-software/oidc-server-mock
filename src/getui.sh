#!/usr/bin/env bash

set -e

COMMIT="cf4546f6dcded593ca1dc08a343c599a4ad8917e"

SOURCE="https://github.com/DuendeArchive/IdentityServer.Quickstart.UI/archive/$COMMIT.zip"
curl -L -o ui.zip "$SOURCE"

unzip -d ui ui.zip

[[ -d Pages ]] || mkdir Pages
[[ -d wwwroot ]] || mkdir wwwroot

cp -r ./ui/IdentityServer.Quickstart.UI-$COMMIT/Pages/* Pages
cp -r ./ui/IdentityServer.Quickstart.UI-$COMMIT/wwwroot/* wwwroot

rm -rf ui ui.zip
