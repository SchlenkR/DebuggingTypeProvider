
function Publish {
    param (
        $TargetFramework
    )

    dotnet publish ./src/SqlClientTest/SqlClientTest.fsproj -f:$TargetFramework -c Release
}

Publish netstandard2.0
Publish netstandard2.1
Publish net461
Publish net6.0
