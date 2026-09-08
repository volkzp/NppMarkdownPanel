param(
    [switch]$SkipRestore
)

$ErrorActionPreference = "Stop"

Get-ChildItem -LiteralPath $PSScriptRoot -Recurse -File | Unblock-File -ErrorAction SilentlyContinue

$vswhere = "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe"
if (-not (Test-Path -LiteralPath $vswhere)) {
    throw "Visual Studio Build Tools were not found. Install MSBuild and the .NET Framework 4.7.2 targeting pack."
}

$msbuildPath = & $vswhere -latest -products * -requires Microsoft.Component.MSBuild -property installationPath
if (-not $msbuildPath) {
    throw "MSBuild was not found by vswhere."
}

$msbuild = Join-Path $msbuildPath 'MSBuild\Current\Bin\MSBuild.exe'
$solution = Join-Path $PSScriptRoot 'NppMarkdownPanel.sln'
$verifyExports = Join-Path $PSScriptRoot 'verify-exports.ps1'

if (-not $SkipRestore) {
    & $msbuild $solution /t:Restore /p:RestorePackagesConfig=true
    if ($LASTEXITCODE -ne 0) {
        throw "NuGet restore failed with exit code $LASTEXITCODE."
    }
}

foreach ($platform in @('x86', 'x64')) {
    & $msbuild $solution /t:Rebuild /p:Configuration=Release /p:Platform=$platform
    if ($LASTEXITCODE -ne 0) {
        throw "MSBuild failed for $platform with exit code $LASTEXITCODE."
    }

    $outputFolder = if ($platform -eq 'x86') { 'Release' } else { 'Release-x64' }
    $pluginDll = Join-Path $PSScriptRoot "NppMarkdownPanel\bin\$outputFolder\NppMarkdownPanel.dll"
    & $verifyExports -DllPath $pluginDll -ExpectedArchitecture $platform
}

Write-Host "Build completed. Both plugin DLLs have six verified Notepad++ exports."
