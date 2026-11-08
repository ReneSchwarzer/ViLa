#requires -Version 5.1
<#
.SYNOPSIS
Builds and pushes the ViLa image for linux/amd64 and linux/arm64.
.EXAMPLE
./docker/publish-multiarch.ps1 -Version 2.0.0 -Suffix alpha
#>
param(
	[string]$Version = '2.0.0',
	[string]$Suffix = 'alpha',
	[string]$Repository = 'rschwarzer/vila',
	[string]$Feed = $(if ($env:WEBEXPRESS_FEED) { $env:WEBEXPRESS_FEED } else { Join-Path $env:USERPROFILE 'Documents\.nuget' })
)

$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$tag = if ($Suffix) { "$Version-$Suffix" } else { $Version }

# The default "docker" driver cannot build multi-platform images.
if (-not (docker buildx ls | Select-String 'multiarch')) {
	docker buildx create --name multiarch --driver docker-container | Out-Null
}

docker buildx build `
	--builder multiarch `
	--platform linux/amd64,linux/arm64 `
	--build-context "nuget=$Feed" `
	-t "${Repository}:$tag" `
	--push `
	$root
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

docker buildx imagetools inspect "${Repository}:$tag"
