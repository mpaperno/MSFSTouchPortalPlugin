# Cleanup
Remove-Item -Path .\MSFSTouchPortalPlugin-Tests\TestResults -Recurse -Force

dotnet sonarscanner begin /k:"MSFSTouchPortalPlugin" /d:sonar.host.url="http://192.168.1.85:6600"  /d:sonar.login="a9fbfa23c79fe20bb51241425e2b314e1cc30271" /d:sonar.cs.opencover.reportsPaths="**/coverage.opencover.xml" /d:sonar.cs.vstest.reportsPaths="**/*.trx"
dotnet build
dotnet test --logger:"trx" --collect:"XPlat Code Coverage" -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=opencover
dotnet sonarscanner end /d:sonar.login="a9fbfa23c79fe20bb51241425e2b314e1cc30271"