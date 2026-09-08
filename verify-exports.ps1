param(
    [Parameter(Mandatory = $true)]
    [string]$DllPath,

    [Parameter(Mandatory = $false)]
    [ValidateSet("x86", "x64")]
    [string]$ExpectedArchitecture
)

$ErrorActionPreference = "Stop"
$requiredExports = @("isUnicode", "setInfo", "getFuncsArray", "messageProc", "getName", "beNotified")
$bytes = [System.IO.File]::ReadAllBytes((Resolve-Path $DllPath).Path)

function Read-UInt16([int]$Offset) {
    return [BitConverter]::ToUInt16($bytes, $Offset)
}

function Read-UInt32([int]$Offset) {
    return [BitConverter]::ToUInt32($bytes, $Offset)
}

function Convert-RvaToOffset([uint32]$Rva, [int]$SectionTableOffset, [int]$SectionCount) {
    for ($index = 0; $index -lt $SectionCount; $index++) {
        $section = $SectionTableOffset + ($index * 40)
        $virtualSize = Read-UInt32 ($section + 8)
        $virtualAddress = Read-UInt32 ($section + 12)
        $rawSize = Read-UInt32 ($section + 16)
        $rawPointer = Read-UInt32 ($section + 20)
        $mappedSize = [Math]::Max([uint32]$virtualSize, [uint32]$rawSize)

        if ($Rva -ge $virtualAddress -and $Rva -lt ($virtualAddress + $mappedSize)) {
            return [int]($rawPointer + ($Rva - $virtualAddress))
        }
    }

    throw ("RVA 0x{0:X8} is outside all PE sections." -f $Rva)
}

function Read-AsciiString([int]$Offset) {
    $end = $Offset
    while ($end -lt $bytes.Length -and $bytes[$end] -ne 0) {
        $end++
    }
    return [Text.Encoding]::ASCII.GetString($bytes, $Offset, $end - $Offset)
}

if ($bytes.Length -lt 256 -or $bytes[0] -ne 0x4D -or $bytes[1] -ne 0x5A) {
    throw "$DllPath is not a valid PE file."
}

$peOffset = [int](Read-UInt32 0x3C)
if ([Text.Encoding]::ASCII.GetString($bytes, $peOffset, 4) -ne "PE`0`0") {
    throw "$DllPath has no valid PE signature."
}

$machine = Read-UInt16 ($peOffset + 4)
$architecture = switch ($machine) {
    0x014C { "x86" }
    0x8664 { "x64" }
    default { "unknown" }
}
if ($ExpectedArchitecture -and $architecture -ne $ExpectedArchitecture) {
    throw "Expected $ExpectedArchitecture, but $DllPath is $architecture."
}

$sectionCount = Read-UInt16 ($peOffset + 6)
$optionalHeaderSize = Read-UInt16 ($peOffset + 20)
$optionalHeader = $peOffset + 24
$optionalMagic = Read-UInt16 $optionalHeader
$dataDirectoryOffset = switch ($optionalMagic) {
    0x010B { 96 }
    0x020B { 112 }
    default { throw ("Unsupported PE optional-header magic: 0x{0:X4}" -f $optionalMagic) }
}
$sectionTable = $optionalHeader + $optionalHeaderSize
$exportDirectoryRva = Read-UInt32 ($optionalHeader + $dataDirectoryOffset)
if ($exportDirectoryRva -eq 0) {
    throw "$DllPath has no PE export directory."
}

$exportDirectory = Convert-RvaToOffset $exportDirectoryRva $sectionTable $sectionCount
$functionCount = Read-UInt32 ($exportDirectory + 20)
$nameCount = Read-UInt32 ($exportDirectory + 24)
$functionsRva = Read-UInt32 ($exportDirectory + 28)
$namesRva = Read-UInt32 ($exportDirectory + 32)
$ordinalsRva = Read-UInt32 ($exportDirectory + 36)

$functions = Convert-RvaToOffset $functionsRva $sectionTable $sectionCount
$names = Convert-RvaToOffset $namesRva $sectionTable $sectionCount
$ordinals = Convert-RvaToOffset $ordinalsRva $sectionTable $sectionCount
$foundExports = @{}

for ($index = 0; $index -lt $nameCount; $index++) {
    $nameRva = Read-UInt32 ($names + ($index * 4))
    $nameOffset = Convert-RvaToOffset $nameRva $sectionTable $sectionCount
    $name = Read-AsciiString $nameOffset
    $ordinalIndex = Read-UInt16 ($ordinals + ($index * 2))

    if ($ordinalIndex -ge $functionCount) {
        throw "Export '$name' has invalid ordinal index $ordinalIndex (function count: $functionCount)."
    }

    $functionRva = Read-UInt32 ($functions + ($ordinalIndex * 4))
    if ($functionRva -eq 0) {
        throw "Export '$name' has a null function address."
    }

    $foundExports[$name] = $functionRva
}

$missingExports = @($requiredExports | Where-Object { -not $foundExports.ContainsKey($_) })
if ($missingExports.Count -gt 0) {
    throw "Missing required Notepad++ exports: $($missingExports -join ', ')."
}

Write-Host "$architecture export validation passed: $($requiredExports -join ', ')."
