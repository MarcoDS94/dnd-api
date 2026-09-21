#!/usr/bin/env bash
set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
export DOTNET_ROOT="$HOME/.dotnet"
export PATH="$DOTNET_ROOT:$PATH"
export DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1
export DOTNET_CLI_HOME="$SCRIPT_DIR/.dotnet-home"
export DOTNET_CLI_TELEMETRY_OPTOUT=1

if [ "$1" = "run" ]; then
    shift
    exec dotnet run --project "$SCRIPT_DIR/src/Dnd.Api" "$@"
fi

if [ "$1" = "ef" ]; then
    shift
    exec dotnet tool run dotnet-ef "$@"
fi

exec dotnet "$@"
