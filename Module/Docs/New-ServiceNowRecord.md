---
external help file: ServiceNow.PowerShell-help.xml
Module Name: ServiceNow.PowerShell
online version: https://github.com/EvotecIT/ServiceNow.NET
schema: 2.0.0
---
# New-ServiceNowRecord
## SYNOPSIS
Creates a new record in a ServiceNow table.

## SYNTAX
### __AllParameterSets
```powershell
New-ServiceNowRecord -BaseUrl <string> -Username <string> -Password <string> -Table <string> -Data <string> [<CommonParameters>]
```

## DESCRIPTION
Sends a JSON payload to the ServiceNow Table API to create the record.

## EXAMPLES

### EXAMPLE 1
```powershell
PS> New-ServiceNowRecord -BaseUrl "https://instance.service-now.com" -Username "user" -Password "pass" -Table "incident" -Data '{"short_description":"Test"}'
```

Creates an incident with a short description.

### EXAMPLE 2
```powershell
PS> New-ServiceNowRecord -BaseUrl "https://instance.service-now.com" -Username "user" -Password "pass" -Table "problem" -Data '{"short_description":"Example"}'
```

Submits a problem record to the ServiceNow instance.

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
JSON payload describing the record to create.

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

### -Table
Name of the table in which to create the record.

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
