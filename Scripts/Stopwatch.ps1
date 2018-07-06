   Function Initialize-Stopwatch {
    param (
        [Parameter(Mandatory=$true)][string]$UserName,
        [Parameter(Mandatory=$true)][string]$StopwatchName,
        [Parameter(Mandatory=$false)][bool]$Start,
        [Parameter(Mandatory=$false)][bool]$Restart,
        [Parameter(Mandatory=$false)][bool]$Stop,
        [Parameter(Mandatory=$false)][bool]$Reset
    )
        $url = "http://localhost:7071/api"

        $body = '{ "UserName": "' + $UserName + '", "StopWatchName": "' + $StopwatchName + '", "Start": "' + $Start + '", "Restart": "' + $Restart + '", "Stop": "' + $Stop + '", "Reset": "' + $Reset +'"}'
        
        $body | ConvertTo-Json

        Invoke-RestMethod -Uri "$url/CreateStopWatch" -Method Post -Body $body 
    }

       Function Get-StopwatchDetails {
    param (
        [Parameter(Mandatory=$true)][string]$UserName,
        [Parameter(Mandatory=$false)][string]$StopwatchName
    )
        $url = "http://localhost:7071/api"

        $body = '{ "UserName": "' + $UserName + '", "StopWatchName": "' + $StopwatchName + '"}'
        
        $body | ConvertTo-Json

        Invoke-RestMethod -Uri "$url/GetStopwatch" -Method Post -Body $body
    }

    Export-ModuleMember -Function Initialize-Stopwatch
    Export-ModuleMember -Function Get-StopwatchDetails