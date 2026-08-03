---
external help file: ServiceNow.PowerShell-help.xml
Module Name: ServiceNow.PowerShell
online version: https://github.com/EvotecIT/ServiceNow.NET
schema: 2.0.0
---
# Remove-ServiceNowRecord
## SYNOPSIS
Deletes a record from a ServiceNow table.

## SYNTAX
### __AllParameterSets
```powershell
Remove-ServiceNowRecord -BaseUrl <string> -Username <string> -Password <string> -Table <string> -SysId <string> [-Force] [<CommonParameters>]
```

## DESCRIPTION
Calls the ServiceNow Table API to remove the specified record.

## EXAMPLES

### EXAMPLE 1
```powershell
PS> Remove-ServiceNowRecord -BaseUrl "https://instance.service-now.com" -Username "user" -Password "pass" -Table "incident" -SysId "abc123"
```

Deletes the specified incident after confirmation.

### EXAMPLE 2
```powershell
PS> Remove-ServiceNowRecord -BaseUrl "https://instance.service-now.com" -Username "user" -Password "pass" -Table "incident" -SysId "abc123" -Force
```

Deletes the incident without prompting for confirmation.

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

### -Force
Suppress confirmation prompts when deleting.

```yaml
Type: SwitchParameter
Parameter Sets: __AllParameterSets
Aliases: None
Possible values:

Required: False
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
Sys_id of the record to remove.

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
