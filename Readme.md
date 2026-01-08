## Make sure .NET SDK 10 is installed
```powershell
dotnet --info
```

## Upgrade .NET version
```powershell
dotnet tool install -g upgrade-assistant
upgrade-assistant upgrade GameStore.Api.csproj --target-tfm net10.0

:: Assigns the env var to the value
setx DOTNET_UPGRADEASSISTANT_TELEMETRY_OPTOUT 1
:: Prints the env var value
echo %DOTNET_UPGRADEASSISTANT_TELEMETRY_OPTOUT%
```

## Display all available project templates
```powershell
dotnet new list
```