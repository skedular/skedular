#!/usr/bin/env bash

set -e
set -x

BASE_DIR="$(cd "$(dirname "${0}")/.." && pwd)"

dotnet tool update jetbrains.resharper.globaltools
dotnet tool update dotnet-ef
dotnet tool update dotnet-stryker
dotnet tool update dotnet-outdated-tool
dotnet tool update hotchocolate.fusion.commandline
dotnet tool update gitversion.tool
dotnet tool update strawberryshake.tools
