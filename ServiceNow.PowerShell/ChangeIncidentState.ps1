# Example script for updating an incident state
# Build the ServiceNow.PowerShell project and adjust the path to the module as needed.
Import-Module ./ServiceNow.PowerShell.dll

Set-ServiceNowRecord -BaseUrl https://instance.service-now.com -Username admin -Password password -Table incident -SysId abc123 -Data '{ "state": "In Progress" }'
