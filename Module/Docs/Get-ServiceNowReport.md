---
external help file: ServiceNow.PowerShell-help.xml
Module Name: ServiceNow.PowerShell
online version: https://github.com/EvotecIT/ServiceNow.NET
schema: 2.0.0
---
# Get-ServiceNowReport
## SYNOPSIS
Retrieves a ServiceNow report.

## SYNTAX
### __AllParameterSets
```powershell
Get-ServiceNowReport -BaseUrl <string> -Username <string> -Password <string> -Report <string> [<CommonParameters>]
```

## DESCRIPTION
Calls the ServiceNow Report API and returns the report data.

## EXAMPLES

### EXAMPLE 1
```powershell
PS> Get-ServiceNowReport -BaseUrl "https://instance.service-now.com" -Username "user" -Password "pass" -Report "incident_metrics"
```

Outputs the data for the specified report.

### EXAMPLE 2
```powershell
PS> Get-ServiceNowReport -BaseUrl "https://instance.service-now.com" -Username "user" -Password "pass" -Report "asset_summary"
```

Gets a summary of assets defined in the instance.

## PARAMETERS

### -BaseUrl
Base URL of the ServiceNow instance.

```yaml
Type: String
Parameter Sets: __AllParameterSets
Aliases: None
Possible values:

Required: True
Position: named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Password
Password used for authentication.

```yaml
Type: String
Parameter Sets: __AllParameterSets
Aliases: None
Possible values:

Required: True
Position: named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Report
Identifier of the report to retrieve.

```yaml
Type: String
Parameter Sets: __AllParameterSets
Aliases: None
Possible values:

Required: True
Position: named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Username
Username used for authentication.

```yaml
Type: String
Parameter Sets: __AllParameterSets
Aliases: None
Possible values:

Required: True
Position: named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

- `None`

## OUTPUTS

- `None`

## RELATED LINKS

- [PowerShell Documentation](https://learn.microsoft.com/powershell/)
- [Project documentation](https://github.com/ServiceNowNET/ServiceNow.NET)
