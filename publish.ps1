
[CmdletBinding(PositionalBinding=$false)]
Param(
  [string]$ProjectName = "MSFSTouchPortalPlugin",
  [string]$DistroName = "MSFS-TouchPortal-Plugin",
  [string]$BinName = $ProjectName,
  [string]$Configuration = "Publish",
  [string]$Platform = "x64",
  [String]$VersionSuffix = "",
  [switch]$FSX = $false,
  [switch]$NoClean = $false,
  [switch]$BuildAgent = $false
)

if ($FSX) {
  $Platform = "FSX"
  $DistroName = "FSX-TouchPortal-Plugin"
  $BinName = "FSXTouchPortalPlugin"
}

$ErrorActionPreference = "Stop"

$CurrentDir = [System.IO.Path]::GetDirectoryName($myInvocation.MyCommand.Path)
$DistFolderPath = "$CurrentDir\packages-dist"
$PluginFilesPath = "$DistFolderPath\$DistroName"
$BinFilesPath = "$PluginFilesPath\dist"

if (Test-Path $PluginFilesPath) {
  Write-Output "Cleaning '$ProjectName' packages-dist folder '$PluginFilesPath'..."
  Remove-Item $PluginFilesPath -Force -Recurse
}

Write-Output "`nPublishing '$ProjectName' component to '$BinFilesPath' ...`n"
dotnet publish "$ProjectName" --output "$BinFilesPath" --configuration $Configuration -p:Platform=$Platform
if ($LastExitCode -ne 0) { throw ("Error " + $LastExitCode) }

Write-Output "`nPublishing '$ProjectName-Generator' component...`n"
dotnet publish "$ProjectName-Generator" --output "$BinFilesPath" --configuration $Configuration -p:Platform=$Platform
if ($LastExitCode -ne 0) { throw ("Error " + $LastExitCode) }

# Run Documentation
Write-Output "`nGenerating entry.tp JSON and Documentation...`n"
#dotnet run --project "$ProjectName-Generator" -c $Configuration -- -o "$PluginFilesPath"  # can't use -p:Platform='
& "$BinFilesPath\$BinName-Generator.exe" -o "$PluginFilesPath"
if ($LastExitCode -ne 0) { throw ("Error " + $LastExitCode) }

# Copy Readme, CHANGELOG, image(s) to publish folder
copy "README.md" "$PluginFilesPath"
copy "CHANGELOG.md" "$PluginFilesPath"
copy "LICENSE" "$PluginFilesPath"
copy "$ProjectName\airplane_takeoff24.png" "$PluginFilesPath"

# Get version
$FileVersion = (Get-Command $BinFilesPath\$BinName.dll).FileVersionInfo.ProductVersion

# Create TPP File
$TppFile = "$DistFolderPath\$DistroName-$FileVersion$VersionSuffix.tpp"
if (Test-Path $TppFile) {
  Remove-Item $TppFile -Force
}
Write-Output "`nGenerating archive '$TppFile'..."
& "C:\Program Files\7-Zip\7z.exe" a "$TppFile" "$PluginFilesPath*" -tzip `-xr!*.tpp `-xr!*.zip
if ($LastExitCode -ne 0) { throw ("Error " + $LastExitCode) }

if (-not $NoClean) {
  Write-Output "`nCleaning '$ProjectName-Generator' component....`n"
  dotnet clean "$ProjectName-Generator" --configuration $Configuration -p:Platform=$Platform /p:ValidateExecutableReferencesMatchSelfContained=false
  Write-Output "`nCleaning '$ProjectName' component....`n"
  dotnet clean "$ProjectName" --configuration $Configuration -p:Platform=$Platform
}

if ($BuildAgent) {
  exit 0
}
