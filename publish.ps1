
[CmdletBinding(PositionalBinding=$false)]
Param(
  [string]$ProjectName = "MSFSTouchPortalPlugin",
  [string]$Configuration = "Publish",
  [string]$Platform = "MSFS",
  [String]$VersionSuffix = "",
  [String]$WikiPath = "../MSFSTouchPortalPlugin.wiki",
  [switch]$FSX = $false,
  [switch]$TPv3 = $true,
  [switch]$TPv4 = $true,
  [switch]$NoClean = $false,
  [switch]$NoTPP = $false,
  [switch]$Quick = $false,
  [switch]$BuildAgent = $false
)

if ($FSX) {
  $Platform = "FSX"
}

if ($Quick) {
  $NoTPP = $true
  $NoClean = $true
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
  Write-Output "`nCleaning '$BinName' dist folder '$PluginFilesPath'..."
  Remove-Item $PluginFilesPath -Force -Recurse
}
if (Test-Path $GenFilesPath) {
  Write-Output "Removing old generator build in '$GenFilesPath'..."
  Remove-Item $GenFilesPath -Force -Recurse
}

if (-not $NoClean) {
  Write-Output "`nCleaning '$BinName' component....`n"
  dotnet clean "$ProjectName" --configuration $Configuration -p:Platform=$Platform
}

Write-Output "`nPublishing '$BinName' component to '$BinFilesPath' ...`n"
dotnet publish "$ProjectName" --output "$BinFilesPath" --configuration $Configuration -p:Platform=$Platform
if ($LastExitCode -ne 0) { throw ("Error " + $LastExitCode) }

# Build json & docs generator in a temporary location; this also builds the plugin as a dependency, into the same folder.
# It doesn't matter which configuration type this is built with, so just use a Release. The platform matters.
Write-Output "`nBuilding '$BinName-Generator' component...`n"
dotnet build "$ProjectName-Generator" --output "$GenFilesPath" --configuration Release -p:Platform=$Platform
if ($LastExitCode -ne 0) { throw ("Error " + $LastExitCode) }

# Run Documentation with output to the plugin dist root
Write-Output "`nGenerating entry.tp JSON and Documentation...`n"
& "$GenFilesPath\$BinName-Generator.exe" -o "$PluginFilesPath" --tpv3 $TPv3 --tpv4 $TPv4
if ($LastExitCode -ne 0) { throw ("Error " + $LastExitCode) }

if ($TPv4 -And $TPv3) {
  Write-Output "`nGenerating entry.tp JSON for TP v3...`n"
  & "$GenFilesPath\$BinName-Generator.exe" -o "$PluginFilesPath" --tpv3 true --tpv4 false -g entry -j TPv3-entry.tp
  if ($LastExitCode -ne 0) { throw ("Error " + $LastExitCode) }
}

# Make a copy of docs in Wiki folder if we know where it is.
if ($WikiPath -ne "" -And (Test-Path $WikiPath)) {
  copy "$PluginFilesPath\Documentation.md" "$WikiPath\Documentation-$Platform.md"
}

# Clean up the whole generator build output
Write-Output "Removing '$GenFilesPath' ..."
Remove-Item $GenFilesPath -Force -Recurse

# Copy supporting docs and icons to publish folder
copy "README.md" "$PluginFilesPath"
copy "CHANGELOG.md" "$PluginFilesPath"
copy "LICENSE" "$PluginFilesPath"
Copy-Item -Path ".\icons" -Destination "$PluginFilesPath" -Filter *.png -Recurse

# Create TPP File
if (-not $NoTPP) {
  # Get version
  $FileVersion = (Get-Command $BinFilesPath\$BinName.dll).FileVersionInfo.ProductVersion
  if ($LastExitCode -ne 0) { throw ("Error " + $LastExitCode) }

  $TppFile = "$DistFolderPath\$DistroName-$FileVersion$VersionSuffix.tpp"
  if (Test-Path $TppFile) {
    Remove-Item $TppFile -Force
  }
  Write-Output "`nGenerating archive '$TppFile'..."

  & "C:\Program Files\7-Zip\7z.exe" a "$TppFile" "$PluginFilesPath*" -tzip `-xr!*.tpp `-xr!*.zip
  if ($LastExitCode -ne 0) { throw ("Error " + $LastExitCode) }
}

if (-not $NoClean) {
  Write-Output "`nCleaning '$BinName' component....`n"
  dotnet clean "$ProjectName" --configuration $Configuration -p:Platform=$Platform
  # also clean the generator build
  Write-Output "`nCleaning '$BinName-Generator' component....`n"
  dotnet clean "$ProjectName-Generator" --configuration Release -p:Platform=$Platform /p:ValidateExecutableReferencesMatchSelfContained=false
}

if ($BuildAgent) {
  exit 0
}
