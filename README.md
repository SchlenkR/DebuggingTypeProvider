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

- XML doc, giving hints about the hosting during compile time.
- When the TP is instanciated (during compile time), the hosting infos are also written to the file given in the TP parameter.
- During runtime, the value of the `HostingInfos` property shows runtime hosting infos.
