Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$repoRoot = Split-Path -Path $PSScriptRoot -Parent
$solutionPath = Join-Path $repoRoot 'src\Pricing.RuleCenter.slnx'

if (-not (Test-Path $solutionPath)) {
    throw "Missing solution file: $solutionPath"
}

if ([string]::IsNullOrWhiteSpace($env:PRICING_ORACLE_CONNECTION_STRING)) {
    throw 'PRICING_ORACLE_CONNECTION_STRING is required to run Oracle integration tests.'
}

dotnet test $solutionPath `
    --no-restore `
    --configuration Release `
    --filter 'Category=OracleIntegration'
