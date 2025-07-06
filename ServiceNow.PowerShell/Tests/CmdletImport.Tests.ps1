Describe 'ServiceNow.PowerShell cmdlets' {
    BeforeAll {
        $modulePath = Join-Path $PSScriptRoot '..' 'bin' 'Release' 'net8.0' 'ServiceNow.PowerShell.dll'
        if (-not (Test-Path $modulePath)) {
            $modulePath = Join-Path $PSScriptRoot '..' 'bin' 'Release' 'net472' 'ServiceNow.PowerShell.dll'
        }
        Import-Module $modulePath -Force
    }

    It 'Get-ServiceNowReport is available' {
        Get-Command Get-ServiceNowReport | Should -Not -BeNullOrEmpty
    }

    It 'Get-ServiceNowRecordList is available' {
        Get-Command Get-ServiceNowRecordList | Should -Not -BeNullOrEmpty
    }

    It 'Remove-ServiceNowRecord is available' {
        Get-Command Remove-ServiceNowRecord | Should -Not -BeNullOrEmpty
    }
}
