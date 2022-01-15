#!/usr/bin/env bash

set -e

case $0 in
    /*)
        cd "$(dirname $0)"
    ;;
    *)
        cd "$(dirname $(pwd)/$0)"
    ;;
esac

output_dir=../Assets/Game.Editor
dotnet build ./MessagePackTools/MessagePackTools.Lib/MessagePackTools.Lib.csproj -c Release -o "$output_dir"
rm ${output_dir}/*.deps.json

