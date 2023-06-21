
function Publish {
    param (
        $TargetFramework
    )

    dotnet publish ./src/SqlClientTest/SqlClientTest.fsproj -f:$TargetFramework -c Release
}

Publish netstandard2.1
Publish net462
