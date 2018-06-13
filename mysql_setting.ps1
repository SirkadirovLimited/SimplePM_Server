# Объявление и инициализация некоторых переменных
$url = "https://raw.githubusercontent.com/SirkadirovTeam/SimplePM_WebApp/master/!DUMPS!/simplepm2.sql"
$output = "$PSScriptRoot\simplepm.sql"
$start_time = Get-Date

Write-Output "Downloading SimplePM database dump file from $($url) to $($output)"

# Запуск скрипта загрузки файла
Invoke-WebRequest -Uri $url -OutFile $output

Write-Output "File downloading completed (or not)"

# Оповещаем пользователя о прошедшем времени
Write-Output "Time taken: $((Get-Date).Subtract($start_time).Seconds) second(s)."
