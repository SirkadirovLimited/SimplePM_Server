cd /d %~dp0

taskkill /F /IM dotnet.exe /T

dotnet simplepm-server.dll