Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$repoRoot = Split-Path -Path $PSScriptRoot -Parent
$sqlRoot = Join-Path $repoRoot 'sql'
$workflowPath = Join-Path $repoRoot '.github\workflows\rule-center-ci.yml'

if (-not (Test-Path $sqlRoot)) {
    throw "Missing sql directory: $sqlRoot"
}

if (-not (Test-Path $workflowPath)) {
    throw "Missing workflow file: $workflowPath"
}

$requiredSqlFiles = @(
    '00-drop-all.sql',
    '01-create-tables.sql',
    '02-init-dict-data.sql',
    '03-init-formula-def.sql',
    '04-import-rules.sql',
    '05-fix-action-type-order.sql',
    '99-verify.sql'
)

foreach ($file in $requiredSqlFiles) {
    $fullPath = Join-Path $sqlRoot $file
    if (-not (Test-Path $fullPath)) {
        throw "Missing required SQL file: $file"
    }
}

$orderedFiles = Get-ChildItem -Path $sqlRoot -File |
    Where-Object { $_.Name -match '^\d{2}-.*\.sql$' } |
    Sort-Object Name

if ($orderedFiles.Count -lt $requiredSqlFiles.Count) {
    throw "Not enough ordered SQL files found under sql directory."
}

$numericPrefixes = $orderedFiles |
    ForEach-Object { [int]$_.BaseName.Substring(0, 2) }

$duplicatePrefixes = @($numericPrefixes | Group-Object | Where-Object { $_.Count -gt 1 })
if ($duplicatePrefixes.Count -gt 0) {
    throw "Duplicate numeric SQL prefixes detected."
}

foreach ($file in $orderedFiles) {
    $content = Get-Content -Path $file.FullName -Raw
    if ([string]::IsNullOrWhiteSpace($content)) {
        throw "SQL file is empty: $($file.Name)"
    }
}

Write-Host "Release asset validation passed."
