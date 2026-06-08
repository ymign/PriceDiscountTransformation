param(
    [string]$ContainerName = 'pricing-oracle',
    [string]$OracleImage = 'container-registry.oracle.com/database/free:latest',
    [string]$OraclePassword = 'Pricing123456',
    [string]$DbUser = 'PRICING',
    [string]$DbPassword = 'Pricing123456',
    [string]$ApiUrl = 'http://127.0.0.1:5293',
    [switch]$ResetDatabase,
    [switch]$SkipSmokeTest
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$repoRoot = Split-Path -Path $PSScriptRoot -Parent
$sqlRoot = Join-Path $repoRoot 'sql'
$apiProject = Join-Path $repoRoot 'src\Pricing.RuleCenter.Api\Pricing.RuleCenter.Api.csproj'
$logRoot = Join-Path $repoRoot 'logs'
$sqlMount = '/opt/pricing-sql'
$serviceName = 'FREEPDB1'
$adminKey = 'admin-key'
$serviceKey = 'service-key'

function Write-Step {
    param([string]$Message)
    Write-Host "==> $Message"
}

function Invoke-OracleSql {
    param(
        [Parameter(Mandatory = $true)][string]$Connection,
        [Parameter(Mandatory = $true)][string]$Sql
    )

    $tempPath = [System.IO.Path]::GetTempFileName()
    try {
        $content = "set serveroutput on`nset define off`n$Sql`nexit`n"
        $utf8NoBom = [System.Text.UTF8Encoding]::new($false)
        [System.IO.File]::WriteAllText($tempPath, $content, $utf8NoBom)
        $command = "type ""$tempPath"" | docker exec -i $ContainerName sqlplus -S ""$Connection"""
        $output = cmd.exe /d /c $command
        if ($LASTEXITCODE -ne 0) {
            throw "sqlplus failed with exit code $LASTEXITCODE.`n$output"
        }

        return ($output | Out-String)
    }
    finally {
        Remove-Item -LiteralPath $tempPath -Force -ErrorAction SilentlyContinue
    }
}

function Get-OracleNumbers {
    param([string]$Sql)

    $output = Invoke-OracleSql -Connection "$DbUser/$DbPassword@$serviceName" -Sql @"
set heading off feedback off pagesize 0 verify off echo off
$Sql
"@
    $values = @([regex]::Matches($output, '^\s*(\d+)\s*$', [System.Text.RegularExpressions.RegexOptions]::Multiline) |
        ForEach-Object { [int64]$_.Groups[1].Value })
    return ,$values
}

function Test-OracleTable {
    param([string]$TableName)

    $numbers = Get-OracleNumbers -Sql "select count(*) from user_tables where table_name = upper('$TableName');"
    $numberArray = @($numbers)
    return ($numberArray.Count -gt 0 -and $numberArray[0] -gt 0)
}

function Start-OracleContainer {
    if (-not (Get-Command docker -ErrorAction SilentlyContinue)) {
        throw 'Docker is required but was not found in PATH.'
    }

    if (-not (Test-Path $sqlRoot)) {
        throw "Missing sql directory: $sqlRoot"
    }

    $existingContainer = docker ps -a --filter "name=^/$ContainerName$" --format '{{.Names}}'
    if ([string]::IsNullOrWhiteSpace($existingContainer)) {
        if ([string]::IsNullOrWhiteSpace((docker images -q $OracleImage))) {
            Write-Step "Pulling Oracle image $OracleImage"
            docker pull $OracleImage | Out-Host
        }

        Write-Step "Creating Oracle container $ContainerName"
        docker run -d `
            --name $ContainerName `
            -p 1521:1521 `
            -e "ORACLE_PWD=$OraclePassword" `
            -v "${sqlRoot}:$sqlMount:ro" `
            $OracleImage | Out-Host
    }
    else {
        $runningContainer = docker ps --filter "name=^/$ContainerName$" --format '{{.Names}}'
        if ([string]::IsNullOrWhiteSpace($runningContainer)) {
            Write-Step "Starting Oracle container $ContainerName"
            docker start $ContainerName | Out-Host
        }
    }
}

function Wait-OracleReady {
    Write-Step 'Waiting for Oracle to accept connections'
    for ($i = 1; $i -le 90; $i++) {
        try {
            Invoke-OracleSql -Connection "sys/$OraclePassword@$serviceName as sysdba" -Sql 'select 1 from dual;' | Out-Null
            return
        }
        catch {
            Start-Sleep -Seconds 5
        }
    }

    throw 'Oracle did not become ready within the expected time.'
}

function Reset-OracleUser {
    if (-not $ResetDatabase) {
        return
    }

    Write-Step "Dropping Oracle user $DbUser"
    Invoke-OracleSql -Connection "sys/$OraclePassword@$serviceName as sysdba" -Sql @"
alter session set container = FREEPDB1;
declare
  v_count number;
begin
  select count(*) into v_count from dba_users where username = upper('$DbUser');
  if v_count > 0 then
    execute immediate 'drop user $DbUser cascade';
  end if;
end;
/
"@ | Out-Host
}

function Ensure-OracleUser {
    Write-Step "Ensuring Oracle user $DbUser"
    Invoke-OracleSql -Connection "sys/$OraclePassword@$serviceName as sysdba" -Sql @"
alter session set container = FREEPDB1;
declare
  v_count number;
begin
  select count(*) into v_count from dba_users where username = upper('$DbUser');
  if v_count = 0 then
    execute immediate 'create user $DbUser identified by "$DbPassword"';
  end if;

  execute immediate 'alter user $DbUser identified by "$DbPassword" account unlock';
  execute immediate 'grant connect, resource to $DbUser';
  execute immediate 'grant create view, create synonym to $DbUser';
  execute immediate 'alter user $DbUser quota unlimited on users';
end;
/
"@ | Out-Host
}

function Invoke-SeedScript {
    param([string]$FileName)

    Write-Step "Running $FileName"
    Invoke-OracleSql -Connection "$DbUser/$DbPassword@$serviceName" -Sql "@$sqlMount/$FileName" | Out-Host
}

function Initialize-FinDiscountSource {
    $finSqlPath = Join-Path $sqlRoot 'FIN_DISCOUNT_FEE.sql'
    if (-not (Test-Path $finSqlPath)) {
        throw "Missing FIN source script: $finSqlPath"
    }

    Write-Step 'Seeding FIN_DISCOUNT_FEE source table'
    Invoke-OracleSql -Connection "$DbUser/$DbPassword@$serviceName" -Sql @"
begin
  execute immediate 'drop table FIN_DISCOUNT_FEE purge';
exception
  when others then null;
end;
/
create table FIN_DISCOUNT_FEE (
  ITEM_CODE     varchar2(50),
  ITEM_NAME     varchar2(200),
  DISCOUNT_RATE varchar2(50),
  TOPPRICE      varchar2(50),
  DISCOUNT_TYPE varchar2(10),
  VALID_STATE   varchar2(10)
);
"@ | Out-Host

    $source = [System.IO.File]::ReadAllText($finSqlPath)
    $pattern = "values\s*\('(?<item>[^']*)',\s*'(?<name>[^']*)',\s*'(?<rate>[^']*)',\s*'(?<top>[^']*)',\s*'(?<type>[^']*)',\s*'(?<state>[^']*)'\);"
    $matches = [regex]::Matches($source, $pattern, [System.Text.RegularExpressions.RegexOptions]::IgnoreCase)
    if ($matches.Count -eq 0) {
        throw 'No FIN_DISCOUNT_FEE rows were parsed from FIN_DISCOUNT_FEE.sql.'
    }

    $insertLines = foreach ($match in $matches) {
        $itemCode = $match.Groups['item'].Value.Replace("'", "''")
        $rate = $match.Groups['rate'].Value.Replace("'", "''")
        $top = $match.Groups['top'].Value.Replace("'", "''")
        $discountType = $match.Groups['type'].Value.Replace("'", "''")
        $validState = $match.Groups['state'].Value.Replace("'", "''")
        "insert into FIN_DISCOUNT_FEE (ITEM_CODE, ITEM_NAME, DISCOUNT_RATE, TOPPRICE, DISCOUNT_TYPE, VALID_STATE) values ('$itemCode', '$itemCode', '$rate', '$top', '$discountType', '$validState');"
    }

    Invoke-OracleSql -Connection "$DbUser/$DbPassword@$serviceName" -Sql (($insertLines + 'commit;') -join "`n") | Out-Host
}

function Approve-LocalPolicies {
    Write-Step 'Approving imported local policies'
    Invoke-OracleSql -Connection "$DbUser/$DbPassword@$serviceName" -Sql @"
update pr_policy_version
   set policy_status = 'APPROVED'
 where policy_status <> 'APPROVED';

insert into pr_policy_review (
    review_id,
    policy_version_id,
    review_status,
    review_stage,
    submitted_by,
    submitted_at,
    reviewed_by,
    reviewed_at,
    review_comment,
    source_checksum
)
select seq_pr_policy_review.nextval,
       pv.policy_version_id,
       'APPROVED',
       'LOCAL_INIT',
       'codex-local',
       sysdate,
       'codex-local',
       sysdate,
       'local runtime package seed approval',
       pv.checksum
  from pr_policy_version pv
 where not exists (
       select 1
         from pr_policy_review pr
        where pr.policy_version_id = pv.policy_version_id
          and pr.review_status = 'APPROVED'
          and pr.source_checksum = pv.checksum
 );

commit;
"@ | Out-Host
}

function Initialize-Database {
    if ((Test-OracleTable -TableName 'PR_RUNTIME_PACKAGE')) {
        $activePackageCount = Get-OracleNumbers -Sql "select count(*) from pr_runtime_package where package_status = 'ACTIVE';"
        $activePackageCountArray = @($activePackageCount)
        if ($activePackageCountArray.Count -gt 0 -and $activePackageCountArray[0] -gt 0) {
            Write-Step 'Database already has an active runtime package, skipping seed import'
            return
        }
    }

    if ((Test-OracleTable -TableName 'PR_POLICY_VERSION')) {
        $policyVersionCount = Get-OracleNumbers -Sql 'select count(*) from pr_policy_version;'
        $policyVersionCountArray = @($policyVersionCount)
        if ($policyVersionCountArray.Count -gt 0 -and $policyVersionCountArray[0] -gt 0) {
            Write-Step 'Database already has imported policies, only preparing approval state'
            Approve-LocalPolicies
            return
        }
    }

    if ((Test-OracleTable -TableName 'PR_RULE_HEADER')) {
        throw 'Database is partially initialized. Re-run this script with -ResetDatabase to rebuild the local PRICING schema.'
    }

    $seedScripts = @(
        '01-create-tables.sql',
        '02-init-dict-data.sql',
        '03-init-formula-def.sql',
        '05-fix-action-type-order.sql',
        '06-config-first-authoring-runtime-schema.sql',
        '07-seed-template-catalog.sql'
    )

    foreach ($script in $seedScripts) {
        Invoke-SeedScript -FileName $script
    }

    Initialize-FinDiscountSource
    Invoke-SeedScript -FileName '04-import-rules.sql'
    Invoke-SeedScript -FileName '08-import-initial-policies.sql'
    Approve-LocalPolicies
}

function Test-ApiHealth {
    try {
        $health = Invoke-RestMethod -Uri "$($ApiUrl.TrimEnd('/'))/health" -TimeoutSec 3
        return ($health.code -eq 0)
    }
    catch {
        return $false
    }
}

function Start-Api {
    if (Test-ApiHealth) {
        Write-Step "API already responds at $ApiUrl"
        return
    }

    if (-not (Test-Path $apiProject)) {
        throw "Missing API project: $apiProject"
    }

    New-Item -ItemType Directory -Path $logRoot -Force | Out-Null
    $stdoutLog = Join-Path $logRoot 'local-api.out.log'
    $stderrLog = Join-Path $logRoot 'local-api.err.log'

    $env:ASPNETCORE_ENVIRONMENT = 'Development'
    $env:ASPNETCORE_URLS = $ApiUrl
    $env:Pricing__OracleConnectionString = "Data Source=127.0.0.1:1521/$serviceName;User Id=$DbUser;Password=$DbPassword;"
    $env:Pricing__EnableAuthorityPriceCheck = 'false'
    $env:Swagger__Enabled = 'true'
    $env:Authentication__ApiKey__Keys__0__Key = $serviceKey
    $env:Authentication__ApiKey__Keys__0__Name = 'local-service'
    $env:Authentication__ApiKey__Keys__0__Roles__0 = 'pricing.service'
    $env:Authentication__ApiKey__Keys__1__Key = $adminKey
    $env:Authentication__ApiKey__Keys__1__Name = 'local-admin'
    $env:Authentication__ApiKey__Keys__1__Roles__0 = 'pricing.admin'
    $env:Authentication__ApiKey__Keys__1__Roles__1 = 'pricing.service'

    Write-Step "Starting API at $ApiUrl"
    $process = Start-Process `
        -FilePath 'dotnet' `
        -ArgumentList @('run', '--project', $apiProject, '--no-launch-profile') `
        -WorkingDirectory $repoRoot `
        -RedirectStandardOutput $stdoutLog `
        -RedirectStandardError $stderrLog `
        -WindowStyle Hidden `
        -PassThru
    Write-Host "API PID: $($process.Id)"

    for ($i = 1; $i -le 60; $i++) {
        if (Test-ApiHealth) {
            return
        }

        Start-Sleep -Seconds 2
    }

    throw "API did not become healthy. Check $stdoutLog and $stderrLog."
}

function Publish-RuntimePackage {
    $history = Invoke-RestMethod `
        -Uri "$($ApiUrl.TrimEnd('/'))/api/pricing/runtime-packages/history?take=1" `
        -Headers @{ 'X-Api-Key' = $adminKey }
    $historyItems = @($history.data)
    if ($historyItems.Count -gt 0 -and $historyItems[0].package_status -eq 'ACTIVE') {
        Write-Step "Runtime package already active: $($historyItems[0].package_id)"
        return
    }

    $policyVersionIds = Get-OracleNumbers -Sql 'select policy_version_id from pr_policy_version order by policy_version_id;'
    if ($policyVersionIds.Count -eq 0) {
        throw 'No policy versions found for runtime package publish.'
    }

    Write-Step "Publishing runtime package with $($policyVersionIds.Count) policy versions"
    $body = @{
        policy_version_ids = $policyVersionIds
        published_by = 'codex-local'
    } | ConvertTo-Json -Depth 5 -Compress

    Invoke-RestMethod `
        -Method Post `
        -Uri "$($ApiUrl.TrimEnd('/'))/api/pricing/runtime-packages/publish" `
        -Headers @{ 'X-Api-Key' = $adminKey } `
        -ContentType 'application/json' `
        -Body $body | ConvertTo-Json -Depth 8 | Write-Host
}

Start-OracleContainer
Wait-OracleReady
Reset-OracleUser
Ensure-OracleUser
Initialize-Database
Start-Api
Publish-RuntimePackage

if (-not $SkipSmokeTest) {
    & (Join-Path $PSScriptRoot 'test-local-api.ps1') -ApiUrl $ApiUrl -AdminApiKey $adminKey -ServiceApiKey $serviceKey
}

Write-Host "Local pricing API is ready: $ApiUrl"
