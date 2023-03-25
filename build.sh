#!/bin/bash

TARGET_VERSION="${1:-1.0.0}"
OUTPUT_DIR="${2:-'/tmp'}"

echo "BUILD VERSION $TARGET_VERSION"

ID=$(docker build --build-arg TARGET_VERSION="$TARGET_VERSION" . -q | cut -d ":" -f 2)
docker run --name tmpbuild "$ID"
docker cp tmpbuild:/App/build/VmChamp "$OUTPUT_DIR"
docker rm tmpbuild
