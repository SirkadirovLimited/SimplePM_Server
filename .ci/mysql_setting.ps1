Write-Output "|||||||||||||||||||||||||||| MYSQL DUMP CONFIGURATION STAGE |||||||||||||||||||||||||||||"

$simplepm_database_dump_url = "https://raw.githubusercontent.com/SirkadirovTeam/SimplePM_WebApp/master/!DUMPS!/simplepm2.sql"
$simplepm_mysql_db_dump_path = "$PSScriptRoot\simplepm.sql"
$start_time = Get-Date

Write-Output "Downloading SimplePM database dump file from $($simplepm_database_dump_url) to $($simplepm_mysql_db_dump_path)"

Invoke-WebRequest -Uri $simplepm_database_dump_url -OutFile $simplepm_mysql_db_dump_path

Write-Output "Time taken: $((Get-Date).Subtract($start_time).Seconds) second(s)."

if (Test-Path $simplepm_mysql_db_dump_path)
{
	
	Write-Output "SimplePM database dump file exists."
	
	# TODO: Load dump to MySQL server
	
}
else
{
	
	Write-Error "SimplePM database dump file not exists!"
	
}

Write-Output "||||||||||||||||||||||||||||||||||||||  STAGE END  ||||||||||||||||||||||||||||||||||||||"