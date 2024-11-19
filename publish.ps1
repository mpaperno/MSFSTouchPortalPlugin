
[CmdletBinding(PositionalBinding=$false)]
Param(
  [string]$ProjectName = "MSFSTouchPortalPlugin",
  [string]$Configuration = "Publish",
  [string]$Platform = "MSFS",
  [String]$VersionSuffix = "",
  [switch]$FSX = $false,
  [switch]$NoClean = $false,
  [switch]$BuildAgent = $false
)

if ($FSX) {
  $Platform = "FSX"
}

$ErrorActionPreference = "Stop"

$DistroName = "${Platform}-TouchPortal-Plugin"
$BinName = "${Platform}TouchPortalPlugin"
$CurrentDir = [System.IO.Path]::GetDirectoryName($myInvocation.MyCommand.Path)
$DistFolderPath = "$CurrentDir\packages-dist"
$PluginFilesPath = "$DistFolderPath\$DistroName"
$BinFilesPath = "$PluginFilesPath\dist"
$GenFilesPath = "$DistFolderPath\Generator"

if (Test-Path $PluginFilesPath) {
  Write-Output "Cleaning '$ProjectName' packages-dist folder '$PluginFilesPath'..."
  Remove-Item $PluginFilesPath -Force -Recurse
}
if (Test-Path $GenFilesPath) {
  Remove-Item $GenFilesPath -Force -Recurse
}

Write-Output "`nPublishing '$ProjectName' component to '$BinFilesPath' ...`n"
dotnet publish "$ProjectName" --output "$BinFilesPath" --configuration $Configuration -p:Platform=$Platform
if ($LastExitCode -ne 0) { throw ("Error " + $LastExitCode) }

Write-Output "`nPublishing '$ProjectName-Generator' component...`n"
#dotnet publish "$ProjectName-Generator" --output "$BinFilesPath" --configuration $Configuration -p:Platform=$Platform
dotnet build "$ProjectName" --output "$GenFilesPath" --configuration Release -p:Platform=$Platform
dotnet build "$ProjectName-Generator" --output "$GenFilesPath" --configuration Release -p:Platform=$Platform
if ($LastExitCode -ne 0) { throw ("Error " + $LastExitCode) }

# Run Documentation
Write-Output "`nGenerating entry.tp JSON and Documentation...`n"
#dotnet run --project "$ProjectName-Generator" -c $Configuration -- -o "$PluginFilesPath"  # can't use -p:Platform='
& "$GenFilesPath\$BinName-Generator.exe" -o "$PluginFilesPath"
if ($LastExitCode -ne 0) { throw ("Error " + $LastExitCode) }

Write-Output "Cleaning '$GenFilesPath' ..."
Remove-Item $GenFilesPath -Force -Recurse

# Copy Readme, CHANGELOG, image(s) to publish folder
copy "README.md" "$PluginFilesPath"
copy "CHANGELOG.md" "$PluginFilesPath"
copy "LICENSE" "$PluginFilesPath"
Copy-Item -Path ".\icons" -Destination "$PluginFilesPath" -Filter *.png -Recurse

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
  dotnet clean "$ProjectName-Generator" --configuration Release -p:Platform=$Platform /p:ValidateExecutableReferencesMatchSelfContained=false
  Write-Output "`nCleaning '$ProjectName' component....`n"
  dotnet clean "$ProjectName" --configuration $Configuration -p:Platform=$Platform
  if ($Configuration -ne "Release") {
    dotnet clean "$ProjectName" --configuration Release -p:Platform=$Platform
  }
}

if ($BuildAgent) {
  exit 0
}
