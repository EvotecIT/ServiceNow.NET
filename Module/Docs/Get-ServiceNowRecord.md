---
external help file: ServiceNow.PowerShell-help.xml
Module Name: ServiceNow.PowerShell
online version: https://github.com/EvotecIT/ServiceNow.NET
schema: 2.0.0
---
# Get-ServiceNowRecord
## SYNOPSIS
Retrieves a single ServiceNow record by its identifier.

## SYNTAX
### __AllParameterSets
```powershell
Get-ServiceNowRecord -BaseUrl <string> -Username <string> -Password <string> -Table <string> -SysId <string> [<CommonParameters>]
```

## DESCRIPTION
Fetches the record from the specified table using the ServiceNow Table API.

## EXAMPLES

### EXAMPLE 1
```powershell
PS> Get-ServiceNowRecord -BaseUrl "https://instance.service-now.com" -Username "user" -Password "pass" -Table "incident" -SysId "abc123"
```

Retrieves the incident with the specified sys_id.

### EXAMPLE 2
```powershell
PS> Get-ServiceNowRecord -BaseUrl "https://instance.service-now.com" -Username "user" -Password "pass" -Table "change_request" -SysId "def456"
```

Outputs the change request record for inspection.

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

### -SysId
Sys_id of the record to retrieve.

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

### -Table
Name of the table containing the record.

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
