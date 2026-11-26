#!/bin/sh
set -eu
envsubst '$IDENTITY_AUTHORITY $API_ENDPOINT $WASM_ORIGIN' < /etc/blazor/appsettings.template.json > /usr/share/nginx/html/appsettings.json
