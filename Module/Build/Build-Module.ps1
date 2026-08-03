Import-Module PSPublishModule -Force -ErrorAction Stop

Build-Module -ModuleName 'ServiceNow.PowerShell' {
    $manifest = [ordered] @{
        ModuleVersion        = '1.0.0'
        CompatiblePSEditions = @('Desktop', 'Core')
        GUID                 = '88486275-7e7a-4f94-95d1-2c15e0a2251d'
        Author               = 'Przemyslaw Klys'
        CompanyName          = 'Evotec'
        Copyright            = "(c) 2011 - $((Get-Date).Year) Przemyslaw Klys @ Evotec. All rights reserved."
        Description          = 'PowerShell cmdlets for working with ServiceNow.'
        Tags                 = @('ServiceNow', 'PowerShell')
        ProjectUri           = 'https://github.com/EvotecIT/ServiceNow.NET'
        PowerShellVersion    = '5.1'
    }
    New-ConfigurationManifest @manifest

    New-ConfigurationDocumentation -Enable -PathReadme 'Docs\Readme.md' -Path 'Docs' -SyncExternalHelpToProjectRoot
    New-ConfigurationImportModule -ImportSelf

    $build = @{
        Enable                        = $true
        SignModule                    = $false
        NETProjectPath                = "$PSScriptRoot\..\..\ServiceNow.PowerShell"
        ResolveBinaryConflicts        = $true
        ResolveBinaryConflictsName    = 'ServiceNow.PowerShell'
        NETProjectName                = 'ServiceNow.PowerShell'
        NETBinaryModule               = 'ServiceNow.PowerShell.dll'
        NETConfiguration              = 'Release'
        NETFramework                  = 'net472', 'net8.0'
        NETSearchClass                = 'ServiceNow.PowerShell.GetServiceNowRecord'
        RefreshPSD1Only               = $false
        NETBinaryModuleDocumentation  = $true
    }
    New-ConfigurationBuild @build

    New-ConfigurationArtefact -Type Unpacked -Enable -Path "$PSScriptRoot\..\Artefacts\Unpacked"
    New-ConfigurationArtefact -Type Packed -Enable -Path "$PSScriptRoot\..\Artefacts\Packed" -IncludeTagName
}
