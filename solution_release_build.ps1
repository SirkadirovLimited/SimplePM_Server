Write-Output "|||||||||||||||||||||||||||||||||||||| BUILD STAGE ||||||||||||||||||||||||||||||||||||||"

Write-Output "Restoring NuGet packages..."
dotnet restore
Write-Output "Restoring NuGet packages completed!"

Write-Output "Removing temporary directories if they exist..."

if (Test-Path "./Build/")
{
	
	Remove-Item -path ./Build/ -recurse -whatif
	Remove-Item -path ./Build/ -recurse
	
}

Write-Output "Temporary directories removed!"

Write-Output "Publishing solution with .NET Core publish feature.."

dotnet publish SimplePM_Server.sln --configuration Release --force --output ./Build/ --verbosity minimal

Write-Output "Building (publishing) process finished!"

Write-Output "||||||||||||||||||||||||||||||||||||||  STAGE END  ||||||||||||||||||||||||||||||||||||||"