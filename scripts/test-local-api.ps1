param(
    [string]$ApiUrl = 'http://127.0.0.1:5293',
    [string]$AdminApiKey = 'admin-key',
    [string]$ServiceApiKey = 'service-key'
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$baseUrl = $ApiUrl.TrimEnd('/')

function Invoke-Api {
    param(
        [Parameter(Mandatory = $true)][string]$Method,
        [Parameter(Mandatory = $true)][string]$Path,
        [hashtable]$Headers = @{},
        [object]$Body = $null
    )

    $arguments = @{
        Method = $Method
        Uri = "$baseUrl$Path"
        Headers = $Headers
        TimeoutSec = 20
    }

    if ($null -ne $Body) {
        $arguments.ContentType = 'application/json'
        $arguments.Body = ($Body | ConvertTo-Json -Depth 12)
    }

    return Invoke-RestMethod @arguments
}

function Assert-True {
    param(
        [bool]$Condition,
        [string]$Message
    )

    if (-not $Condition) {
        throw $Message
    }
}

$health = Invoke-Api -Method 'GET' -Path '/health'
Assert-True -Condition ($health.code -eq 0) -Message 'Health check did not return success.'
Assert-True -Condition ($health.data.status -eq 'Healthy') -Message "API health status is $($health.data.status)."
Write-Host "Health: $($health.data.status)"

$unauthorized = $false
try {
    Invoke-Api -Method 'GET' -Path '/api/pricing/dicts/types' | Out-Null
}
catch {
    $response = $_.Exception.Response
    if ($null -ne $response -and [int]$response.StatusCode -eq 401) {
        $unauthorized = $true
    }
}
Assert-True -Condition $unauthorized -Message 'API key authentication check did not return 401.'
Write-Host 'Auth: 401 without API key'

$history = Invoke-Api `
    -Method 'GET' `
    -Path '/api/pricing/runtime-packages/history?take=1' `
    -Headers @{ 'X-Api-Key' = $AdminApiKey }
Assert-True -Condition ($history.data.Count -gt 0) -Message 'No runtime package history found.'
Assert-True -Condition ($history.data[0].package_status -eq 'ACTIVE') -Message 'Latest runtime package is not ACTIVE.'
Write-Host "RuntimePackage: $($history.data[0].package_id) $($history.data[0].package_status)"

$businessTime = (Get-Date).Date.AddHours(10).ToString('yyyy-MM-ddTHH:mm:ss')
$special = Invoke-Api `
    -Method 'GET' `
    -Path "/api/pricing/items/F00000205647/special-flag?charge_scene=OUTPATIENT&business_charge_time=$businessTime" `
    -Headers @{ 'X-Api-Key' = $ServiceApiKey }
Assert-True -Condition ($special.data.is_special -eq $true) -Message 'Special flag did not match F00000205647.'
Assert-True -Condition ($special.data.runtime_package_id -gt 0) -Message 'Special flag did not return runtime package metadata.'
Write-Host "SpecialFlag: item=F00000205647 is_special=$($special.data.is_special) runtime_package=$($special.data.runtime_package_id)"

$requestNo = "SIM_LOCAL_$((Get-Date).ToString('yyyyMMddHHmmssfff'))"
$simulateBody = @{
    source_system = 'HIS'
    patient_id = 'P_LOCAL_001'
    visit_id = 'V_LOCAL_001'
    charge_scene = 'OUTPATIENT'
    charge_dept_code = '1001'
    business_charge_time = $businessTime
    business_request_no = $requestNo
    operator_id = 'codex'
    items = @(
        @{
            item_request_no = '1'
            charge_detail_no = "D_$requestNo"
            item_code = 'F00000205647'
            item_name = 'local RF test item'
            input_qty = 2
            unit = 'EACH'
            unit_price = 100
            business_charge_time = $businessTime
        }
    )
}

$simulate = Invoke-Api `
    -Method 'POST' `
    -Path '/api/pricing/calculate/simulate' `
    -Headers @{ 'X-Api-Key' = $ServiceApiKey } `
    -Body $simulateBody
Assert-True -Condition ($simulate.code -eq 0) -Message 'Simulate did not return success.'
Assert-True -Condition ($simulate.data.is_special_item -eq $true) -Message 'Simulate did not mark item as special.'
Assert-True -Condition ($simulate.data.matched_runtime_rule_ids.Count -gt 0) -Message 'Simulate did not match runtime rules.'
Assert-True -Condition ([decimal]$simulate.data.total_original_amount -eq 200) -Message 'Unexpected original amount.'
Assert-True -Condition ([decimal]$simulate.data.total_final_amount -eq 100) -Message 'Unexpected final amount.'
Assert-True -Condition ([decimal]$simulate.data.total_discount_amount -eq 100) -Message 'Unexpected discount amount.'

Write-Host "Simulate: request=$requestNo original=$($simulate.data.total_original_amount) final=$($simulate.data.total_final_amount) discount=$($simulate.data.total_discount_amount) matched_runtime_rules=$($simulate.data.matched_runtime_rule_ids -join ',')"
