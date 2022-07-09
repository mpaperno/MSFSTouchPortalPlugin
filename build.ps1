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

Write-Information -MessageData "Restoring 'MSFSTouchPortalPlugin' component....`n" -InformationAction Continue
dotnet restore "MSFSTouchPortalPlugin"
dotnet restore "MSFSTouchPortalPlugin.Tests"

Write-Information -MessageData "Building 'MSFSTouchPortalPlugin' component...`n" -InformationAction Continue
dotnet build "MSFSTouchPortalPlugin" --configuration $Configuration

Write-Information "Cleaning 'MSFSTouchPortalPlugin' packages-dist folder..." -InformationAction Continue
$CurrentDir = [System.IO.Path]::GetDirectoryName($myInvocation.MyCommand.Path)
$DistFolderPath = "$CurrentDir\packages-dist"
if (Test-Path $DistFolderPath) {
  Remove-Item $DistFolderPath -Force -Recurse
}

Write-Information "Publishing 'MSFSTouchPortalPlugin' component...`n" -InformationAction Continue
dotnet publish "MSFSTouchPortalPlugin" --output $DistFolderPath --configuration $Configuration $VersionSuffixCommand $VersionSuffix -r "win-x64" --self-contained true

if ($IsBuildAgent) {
  exit 0
}
