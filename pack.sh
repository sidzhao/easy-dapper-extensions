#!/bin/bash
rm -rf nupkgs
dotnet pack --force --include-symbols --configuration release --output nupkgs