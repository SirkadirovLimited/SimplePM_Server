cd /d %~dp0

taskkill /F /IM dotnet.exe /T

dotnet SimplePM_Server.dll