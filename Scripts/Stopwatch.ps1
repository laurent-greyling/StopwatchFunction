﻿   Function Initialize-Stopwatch {
    param (
        [Parameter(Mandatory=$true)][string]$UserName,
        [Parameter(Mandatory=$true)][string]$StopwatchName
    )            
        $url = "http://localhost:7071/api"

        $body = '{ "UserName": "' + $UserName + '", "StopWatchName": "' + $StopwatchName + '"}'
        
        $body | ConvertTo-Json

        Invoke-RestMethod -Uri "$url/CreateStopWatch" -Method Post -Body $body 
    }

    Export-ModuleMember -Function Initialize-Stopwatch