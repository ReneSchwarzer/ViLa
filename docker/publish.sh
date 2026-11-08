#!/usr/bin/env bash
# Builds and pushes the ViLa image for linux/amd64 and linux/arm64.
# Usage: ./docker/publish-multiarch.sh [version] [suffix] [repository]
# Example: ./docker/publish-multiarch.sh 2.0.0 alpha rschwarzer/vila
set -euo pipefail

VERSION="${1:-2.0.0}"
SUFFIX="${2-alpha}"
REPOSITORY="${3:-rschwarzer/vila}"
FEED="${WEBEXPRESS_FEED:-$HOME/Documents/.nuget}"

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
TAG="$VERSION"
if [ -n "$SUFFIX" ]; then
	TAG="$VERSION-$SUFFIX"
fi

# The default "docker" driver cannot build multi-platform images.
if ! docker buildx ls | grep -q '^multiarch'; then
	docker buildx create --name multiarch --driver docker-container >/dev/null
fi

docker buildx build \
	--builder multiarch \
	--platform linux/amd64,linux/arm64 \
	--build-context "nuget=$FEED" \
	-t "$REPOSITORY:$TAG" \
	--push \
	"$ROOT"

docker buildx imagetools inspect "$REPOSITORY:$TAG"
