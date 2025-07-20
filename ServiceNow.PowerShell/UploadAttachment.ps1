# Example script for uploading an attachment to a record
# Build the ServiceNow.PowerShell project and adjust the path to the module as needed.
Import-Module ./ServiceNow.PowerShell.dll

Add-ServiceNowAttachment -BaseUrl https://instance.service-now.com -Username admin -Password password -Table incident -SysId abc123 -FilePath ./example.txt
