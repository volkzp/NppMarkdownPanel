$ErrorActionPreference = "Stop"

$releaseRoot = Join-Path $PSScriptRoot 'Release'
if (Test-Path -LiteralPath $releaseRoot) {
    Remove-Item -LiteralPath $releaseRoot -Recurse -Force
}
New-Item -ItemType Directory -Path $releaseRoot | Out-Null

function Copy-ReleaseDirectory([string]$source, [string]$destination) {
    if (Test-Path -LiteralPath $source) {
        Copy-Item -LiteralPath $source -Destination $destination -Recurse -Force
    }
}

function New-ReleaseZip([string]$platform, [string]$outputFolder) {
    $pluginDll = Join-Path $PSScriptRoot "NppMarkdownPanel\bin\$outputFolder\NppMarkdownPanel.dll"
    if (-not (Test-Path -LiteralPath $pluginDll)) {
        throw "Missing $pluginDll. Run .\build.ps1 first."
    }

    & (Join-Path $PSScriptRoot 'verify-exports.ps1') -DllPath $pluginDll -ExpectedArchitecture $platform

    $staging = Join-Path $releaseRoot "staging-$platform"
    $lib = Join-Path $staging 'lib'
    New-Item -ItemType Directory -Path $lib -Force | Out-Null

    Copy-Item -LiteralPath $pluginDll -Destination (Join-Path $staging 'NppMarkdownPanel.dll')
    Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'README.md') -Destination $staging
    Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'License.txt') -Destination $staging
    Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'THIRD-PARTY-NOTICES.md') -Destination $staging
    Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'NppMarkdownPanel\style.css') -Destination $staging
    Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'NppMarkdownPanel\style-dark.css') -Destination $staging
    Copy-ReleaseDirectory (Join-Path $PSScriptRoot 'help') (Join-Path $staging 'help')
    Copy-ReleaseDirectory (Join-Path $PSScriptRoot 'NppMarkdownPanel\mathjax') (Join-Path $staging 'mathjax')
    Copy-ReleaseDirectory (Join-Path $PSScriptRoot 'NppMarkdownPanel\katex') (Join-Path $staging 'katex')
    Copy-ReleaseDirectory (Join-Path $PSScriptRoot 'NppMarkdownPanel\localization') (Join-Path $staging 'localization')

    # The helper projects target AnyCPU, so both plugin architectures use the
    # same Release output. Only NppMarkdownPanel.dll has separate x86/x64 paths.
    foreach ($project in @('MarkdigWrapper', 'PanelCommon', 'Webview2Viewer')) {
        $projectOutput = Join-Path $PSScriptRoot "$project\bin\Release"
        Get-ChildItem -LiteralPath $projectOutput -Filter '*.dll' -File | Copy-Item -Destination $lib -Force
    }
    Copy-ReleaseDirectory (Join-Path $PSScriptRoot 'Webview2Viewer\bin\Release\runtimes') (Join-Path $lib 'runtimes')

    $version = (Get-Item -LiteralPath $pluginDll).VersionInfo.FileVersion
    $zipPath = Join-Path $releaseRoot "NppMarkdownPanel-$version-$platform.zip"
    Compress-Archive -Path (Join-Path $staging '*') -DestinationPath $zipPath -Force
    Remove-Item -LiteralPath $staging -Recurse -Force
    Write-Host "Created $zipPath"
}

New-ReleaseZip 'x86' 'Release'
New-ReleaseZip 'x64' 'Release-x64'
