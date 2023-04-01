#!/bin/bash

# exit on fail
set -e

# set colors
RED="\033[0;31m"
GREEN="\033[0;32m"
BLUE="\033[0;34m"
BOLD_BLUE='\033[1;34m'
NC='\033[0m'

# check shell args
TARGET_VERSION="${1:-1.0.0}"
if [[ ! "$TARGET_VERSION" =~ ^([0-9].){2}[0-9] ]]; then
    echo -e "${RED}❌ ${TARGET_VERSION} is not a valid version. Please use semantic versioning (e.g. \"1.2.3\")${NC}"
    exit 1
fi
OUTPUT_DIR="${2:-build}"
if [ ! -d "$OUTPUT_DIR" ]; then
    mkdir -p "$OUTPUT_DIR"
fi

OUTPUT_DIR_REALPATH=$(realpath "$OUTPUT_DIR")

# build image
echo -e "${BOLD_BLUE}ℹ️ Build version: ${TARGET_VERSION}${NC}"
echo -e "${BOLD_BLUE}ℹ️ Output dir: ${OUTPUT_DIR_REALPATH}/${NC}"
echo ""

echo -ne "${BLUE}🏗️ Docker build image...${NC}"
docker build --build-arg TARGET_VERSION="$TARGET_VERSION" -t vmchamp:latest -q . > /dev/null
echo -e "${GREEN} done${NC}"

echo -ne "${BLUE}🏗️ Docker create container...${NC}"
docker create --name vmchamp -q vmchamp:latest exit 0 > /dev/null
echo -e "${GREEN} done${NC}"

echo -ne "${BLUE}🏗️ Docker copy binary...${NC}"
docker cp -q vmchamp:/VmChamp "$OUTPUT_DIR/VmChamp" > /dev/null
echo -e "${GREEN} done${NC}"

echo -ne "${BLUE}🏗️ Docker remove container...${NC}"
docker rm -f vmchamp > /dev/null
echo -e "${GREEN} done${NC}"

echo ""
echo -e "${GREEN}✅ Build successful${NC} - Binary available at $OUTPUT_DIR_REALPATH/VmChamp"
