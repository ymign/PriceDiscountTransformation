param(
    [Parameter(Mandatory = $true)]
    [string]$PackageDirectory,

    [Parameter(Mandatory = $true)]
    [string]$TargetDirectory,

    [string]$BackupDirectory
)

$ErrorActionPreference = "Stop"

$packagePath = (Resolve-Path -LiteralPath $PackageDirectory).Path

if (-not (Test-Path -LiteralPath $TargetDirectory)) {
    New-Item -ItemType Directory -Path $TargetDirectory | Out-Null
}

$targetPath = (Resolve-Path -LiteralPath $TargetDirectory).Path

if ([string]::IsNullOrWhiteSpace($BackupDirectory)) {
    $BackupDirectory = Join-Path $targetPath "pricing-agent-backup"
}

if (-not (Test-Path -LiteralPath $BackupDirectory)) {
    New-Item -ItemType Directory -Path $BackupDirectory | Out-Null
}

$stamp = Get-Date -Format "yyyyMMddHHmmss"
$currentBackup = Join-Path $BackupDirectory $stamp
New-Item -ItemType Directory -Path $currentBackup | Out-Null

$files = @(
    "HIS.Pricing.Client.dll",
    "HIS.Pricing.Client.pdb",
    "Newtonsoft.Json.dll",
    "pricing-agent.config"
)

foreach ($file in $files) {
    $targetFile = Join-Path $targetPath $file
    if (Test-Path -LiteralPath $targetFile) {
        Copy-Item -LiteralPath $targetFile -Destination $currentBackup -Force
    }
}

foreach ($file in $files) {
    $sourceFile = Join-Path $packagePath $file
    if (Test-Path -LiteralPath $sourceFile) {
        Copy-Item -LiteralPath $sourceFile -Destination $targetPath -Force
    }
}

Write-Host "PricingAgent installed to $targetPath"
Write-Host "Backup saved to $currentBackup"
