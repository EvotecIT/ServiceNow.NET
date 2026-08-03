---
external help file: ServiceNow.PowerShell-help.xml
Module Name: ServiceNow.PowerShell
online version: https://github.com/EvotecIT/ServiceNow.NET
schema: 2.0.0
---
# Set-ServiceNowRecord
## SYNOPSIS
Updates an existing ServiceNow record.

## SYNTAX
### __AllParameterSets
```powershell
Set-ServiceNowRecord -BaseUrl <string> -Username <string> -Password <string> -Table <string> -SysId <string> -Data <string> [<CommonParameters>]
```

## DESCRIPTION
Applies the provided JSON payload to the record through the ServiceNow Table API.

## EXAMPLES

### EXAMPLE 1
```powershell
PS> Set-ServiceNowRecord -BaseUrl "https://instance.service-now.com" -Username "user" -Password "pass" -Table "incident" -SysId "abc123" -Data '{"short_description":"Updated"}'
```

Changes the short description of the incident.

### EXAMPLE 2
```powershell
PS> Set-ServiceNowRecord -BaseUrl "https://instance.service-now.com" -Username "user" -Password "pass" -Table "change_request" -SysId "def456" -Data '{"priority":"2"}'
```

Sets the priority field on the change request.

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

### -Data
JSON payload describing the updates to apply.

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
Sys_id of the record to update.

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
