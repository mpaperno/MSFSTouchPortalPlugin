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

Write-Host "Restoring 'MSFSTouchPortalPlugin' component....`n" -foregroundcolor "Magenta"
dotnet restore "MSFSTouchPortalPlugin"
dotnet restore "MSFSTouchPortalPlugin.Tests"

Write-Host "Building 'MSFSTouchPortalPlugin' component...`n" -foregroundcolor "Magenta"
dotnet build "MSFSTouchPortalPlugin" --configuration $Configuration

Write-Host "Cleaning 'MSFSTouchPortalPlugin' packages-dist folder..." -foregroundcolor "Magenta"
$CurrentDir = [System.IO.Path]::GetDirectoryName($myInvocation.MyCommand.Path)
$DistFolderPath = "$CurrentDir\packages-dist"
if (Test-Path $DistFolderPath) {
  Remove-Item $DistFolderPath -Force -Recurse
}

Write-Host "Publishing 'MSFSTouchPortalPlugin' component...`n" -foregroundcolor "Magenta"
dotnet publish "MSFSTouchPortalPlugin" --output $DistFolderPath --configuration $Configuration $VersionSuffixCommand $VersionSuffix -r "win-x64" --self-contained true

if ($IsBuildAgent) {
  exit 0
}
