Param(
  [Parameter(Position = 0)]
  [Boolean]$IsBuildAgent = $false,
  [Parameter(Position = 1)]
  [String]$Configuration = "Release",
  [Parameter(Position = 2)]
  [String]$VersionSuffix = ""
)

if((-Not ($IsBuildAgent)) -And ([string]::IsNullOrEmpty($VersionSuffix))) {
  $VersionSuffix = "1"
}

$VersionSuffixCommand = ""
if(-Not ([string]::IsNullOrEmpty($VersionSuffix))) {
  $VersionSuffixCommand = "--version-suffix"
}

Write-Information "Restoring 'MSFSTouchPortalPlugin' component....`n" -InformationAction Continue
dotnet restore "MSFSTouchPortalPlugin"
dotnet restore "MSFSTouchPortalPlugin.Tests"

Write-Information "Building 'MSFSTouchPortalPlugin' component...`n" -InformationAction Continue
dotnet build "MSFSTouchPortalPlugin" --configuration $Configuration -p:Platform=x64

Write-Information "Cleaning 'MSFSTouchPortalPlugin' packages-dist folder..." -InformationAction Continue
$CurrentDir = [System.IO.Path]::GetDirectoryName($myInvocation.MyCommand.Path)
$DistFolderPath = "$CurrentDir\packages-dist"
if (Test-Path $DistFolderPath) {
  Remove-Item $DistFolderPath -Force -Recurse
}

Write-Information "Publishing 'MSFSTouchPortalPlugin' component...`n" -InformationAction Continue
dotnet publish "MSFSTouchPortalPlugin" --output "$DistFolderPath\MSFS-TouchPortal-Plugin\dist" --configuration $Configuration -p:Platform=x64 $VersionSuffixCommand $VersionSuffix -r "win-x64" --self-contained true

# Run Documentation
dotnet run -p "MSFSTouchPortalPlugin-Generator" "$DistFolderPath\MSFS-TouchPortal-Plugin"

# Copy Entry.tp, Readme, Documentation, CHANGELOG to publish
copy "README.md" "$DistFolderPath\MSFS-TouchPortal-Plugin"
copy "CHANGELOG.md" "$DistFolderPath\MSFS-TouchPortal-Plugin"
copy "airplane_takeoff24.png" "$DistFolderPath\MSFS-TouchPortal-Plugin"

# Get version
$FileVersion = (Get-Command $DistFolderPath\MSFS-TouchPortal-Plugin\dist\MSFSTouchPortalPlugin.dll).FileVersionInfo.FileVersion

# Create TPP File
#Compress-Archive -Path "$DistFolderPath\MSFS-TouchPortal-Plugin" -DestinationPath "$DistFolderPath\MSFS-TouchPortal-Plugin.zip"
#Rename-Item -Path "$DistFolderPath\MSFS-TouchPortal-Plugin.zip" -NewName "MSFS-TouchPortal-Plugin.tpp"
& "C:\Program Files\7-Zip\7z.exe" a $DistFolderPath\MSFS-TouchPortal-Plugin-$FileVersion.tpp "$DistFolderPath\*" -r -tzip

if ($IsBuildAgent) {
  exit 0
}
