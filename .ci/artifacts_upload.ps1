Write-Output "||||||||||||||||||||||||||||||| ARTIFACTS PACKAGING STAGE |||||||||||||||||||||||||||||||"

$zip_output_path = "../Build/SimplePM_Server-build.zip"
$zip_contents_directory = "../Build/"

7z a -tzip -mx=9 -bd $zip_output_path $zip_contents_directory

Push-AppveyorArtifact $zip_output_path -Verbosity Normal

Write-Output "||||||||||||||||||||||||||||||||||||||  STAGE END  ||||||||||||||||||||||||||||||||||||||"