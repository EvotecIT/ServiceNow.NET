$baseUrl = "https://instance.service-now.com"
$username = "admin"
$password = "password"
$sysId = "example_incident"

# Assign incident
Set-ServiceNowRecord -BaseUrl $baseUrl -Username $username -Password $password -Table incident -SysId $sysId -Data '{"state":"In Progress","assigned_to":"user_sys_id"}'

# Resolve incident
Set-ServiceNowRecord -BaseUrl $baseUrl -Username $username -Password $password -Table incident -SysId $sysId -Data '{"state":"Resolved"}'

# Close incident
Set-ServiceNowRecord -BaseUrl $baseUrl -Username $username -Password $password -Table incident -SysId $sysId -Data '{"state":"Closed"}'
