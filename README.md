# DebuggingTp

Very simple F# type provider for all kinds of debugging purpose.

## Hints

- Develop the type provider in `./src/DebuggingTp.sln`
- Run `./publishTypeProviderLocal.ps1` to publish the TP locally to the test project (`./src/ProviderTests`). The publish folder is: `./src/ProviderTests/.tpPublish`.
- Open a new devenv instance or another IDE, and open `./src/ProviderTest/ProviderTest.sln`

- Instanciate a type:

```fsharp
let [<Literal>] LogFileName = "c:/temp/DebuggingTp.log"
type DebuggingProviderType = DebuggingTp.DebuggingTp<LogFileName>
```

The `DebuggingProviderType` has a `HostingInfos` static property with

- XML doc, giving hints about the hosting during design time.
- When the TP is instanciated (during design time), the hosting infos are also written to the file given in the TP parameter.
- During runtime, the value of the `HostingInfos` property shows runtime hosting infos.

## SQL Client

Use an arbitrary DB and pass a CS + a query resulting in a scalar to the provider:

```fsharp
let [<Literal>] Cs = 
    $"""
        Server=(localdb)\MSSQLLocalDB;
        Database=Demo;
        User Id=DebuggingTp;
        Password=DebuggingTp;
    """
let [<Literal>] Sql = "select count(*) from DemoTable"
type TSql = DebuggingTp.SqlClientProvider<Cs, Sql>
```

...and start playing with MsSqlClient (putting files / path / setting probing paths to .tpPublish)

## Test Cases

Design Time:

- VS 2022 (.Net FW)
- FSI (.Net FW + dotnet)
- Ionide (both?)
- Rider (*)

Runtime
