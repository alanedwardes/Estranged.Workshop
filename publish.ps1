Remove-Item publish -Recurse

function Publish-Workshop { dotnet publish src/Estranged.Workshop/Estranged.Workshop.csproj --self-contained --runtime $args --configuration Release --output $PSScriptRoot/publish/$args }

Publish-Workshop linux-x64
Publish-Workshop osx-x64
Publish-Workshop win-x64