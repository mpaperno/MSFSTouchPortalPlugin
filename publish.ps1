
[CmdletBinding(PositionalBinding=$false)]
Param(
  [string]$ProjectName = "MSFSTouchPortalPlugin",
  [string]$DistroName = "MSFS-TouchPortal-Plugin",
  [string]$Configuration = "Publish",
  [string]$Platform = "x64",
  [String]$VersionSuffix = "",
  [switch]$Clean = $false,
  [switch]$BuildAgent = $false
)

$CurrentDir = [System.IO.Path]::GetDirectoryName($myInvocation.MyCommand.Path)
$DistFolderPath = "$CurrentDir\packages-dist"
$PluginFilesPath = "$DistFolderPath\$DistroName"
$BinFilesPath = "$PluginFilesPath\dist"

$VersionSuffixCommand = ""
if(-Not ([string]::IsNullOrEmpty($VersionSuffix))) {
  $VersionSuffixCommand = "--version-suffix"
}

if (Test-Path $PluginFilesPath) {
  Write-Information "Cleaning '$ProjectName' packages-dist folder '$PluginFilesPath'..." -InformationAction Continue
  Remove-Item $PluginFilesPath -Force -Recurse
}

Write-Information "`nPublishing '$ProjectName' component to '$BinFilesPath' ...`n" -InformationAction Continue
dotnet publish "$ProjectName" --output "$BinFilesPath" --configuration $Configuration -p:Platform=$Platform $VersionSuffixCommand $VersionSuffix

Write-Information "`nPublishing '$ProjectName-Generator' component...`n" -InformationAction Continue
dotnet publish "$ProjectName-Generator" --output "$BinFilesPath" --configuration $Configuration -p:Platform=$Platform

# Run Documentation
Write-Information "`nGenerating entry.tp JSON and Documentation..." -InformationAction Continue
#dotnet run -p "$ProjectName-Generator" -o "$PluginFilesPath"
& "$BinFilesPath\$ProjectName-Generator.exe" -o "$PluginFilesPath"

# Copy Readme, CHANGELOG, image(s) to publish folder
copy "README.md" "$PluginFilesPath"
copy "CHANGELOG.md" "$PluginFilesPath"
copy "airplane_takeoff24.png" "$PluginFilesPath"

# Get version
$FileVersion = (Get-Command $BinFilesPath\$ProjectName.dll).FileVersionInfo.ProductVersion

# Create TPP File
$TppFile = "$DistFolderPath\$DistroName-$FileVersion.tpp"
if (Test-Path $TppFile) {
  Remove-Item $TppFile -Force
}
& "C:\Program Files\7-Zip\7z.exe" a "$TppFile" "$DistFolderPath\*" -tzip `-xr!*.tpp

if ($Clean) {
  Write-Information "`nCleaning '$ProjectName-Generator' component....`n" -InformationAction Continue
  dotnet clean "$ProjectName-Generator" --configuration $Configuration -p:Platform=$Platform -r "win-$Platform" /p:ValidateExecutableReferencesMatchSelfContained=false
  Write-Information "`nCleaning '$ProjectName' component....`n" -InformationAction Continue
  dotnet clean "$ProjectName" --configuration $Configuration -p:Platform=$Platform -r "win-$Platform"
}

if ($BuildAgent) {
  exit 0
}
