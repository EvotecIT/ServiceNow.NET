---
external help file: ServiceNow.PowerShell-help.xml
Module Name: ServiceNow.PowerShell
online version: https://github.com/EvotecIT/ServiceNow.NET
schema: 2.0.0
---
# Get-ServiceNowRecordList
## SYNOPSIS
Streams records from a ServiceNow table.

## SYNTAX
### __AllParameterSets
```powershell
Get-ServiceNowRecordList -BaseUrl <string> -Username <string> -Password <string> -Table <string> [-BatchSize <int>] [<CommonParameters>]
```

## DESCRIPTION
Retrieves records in batches using the ServiceNow Table API.

## EXAMPLES

### EXAMPLE 1
```powershell
PS> Get-ServiceNowRecordList -BaseUrl "https://instance.service-now.com" -Username "user" -Password "pass" -Table "incident"
```

The cmdlet writes each incident record to the pipeline.

### EXAMPLE 2
```powershell
PS> Get-ServiceNowRecordList -BaseUrl "https://instance.service-now.com" -Username "user" -Password "pass" -Table "cmdb_ci" -BatchSize 50
```

Streams configuration item records fifty at a time.

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

### -BatchSize
Number of records to retrieve per batch.

```yaml
Type: Int32
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

### -Table
Name of the table to query.

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
