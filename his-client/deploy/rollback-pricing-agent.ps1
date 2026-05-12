param(
    [Parameter(Mandatory = $true)]
    [string]$TargetDirectory,

    [string]$BackupDirectory,

    [string]$BackupName
)

$ErrorActionPreference = "Stop"

$targetPath = (Resolve-Path -LiteralPath $TargetDirectory).Path

if ([string]::IsNullOrWhiteSpace($BackupDirectory)) {
    $BackupDirectory = Join-Path $targetPath "pricing-agent-backup"
}

$backupRoot = (Resolve-Path -LiteralPath $BackupDirectory).Path

if ([string]::IsNullOrWhiteSpace($BackupName)) {
    $backup = Get-ChildItem -LiteralPath $backupRoot -Directory |
        Sort-Object Name -Descending |
        Select-Object -First 1
    if ($null -eq $backup) {
        throw "No PricingAgent backup found in $backupRoot"
    }
    $backupPath = $backup.FullName
}
else {
    $backupPath = Join-Path $backupRoot $BackupName
    if (-not (Test-Path -LiteralPath $backupPath)) {
        throw "Backup not found: $backupPath"
    }
}

Get-ChildItem -LiteralPath $backupPath -File | ForEach-Object {
    Copy-Item -LiteralPath $_.FullName -Destination $targetPath -Force
}

Write-Host "PricingAgent rolled back from $backupPath to $targetPath"
