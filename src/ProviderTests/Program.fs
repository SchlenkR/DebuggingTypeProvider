module Program

let [<Literal>] LogFileName = "c:/temp/DebuggingTp.log"
type DebuggingProviderType = DebuggingTp.DebuggingTp<LogFileName>
     
[<EntryPoint>]
let main _ =
    printfn "%s" (DebuggingProviderType.HostingInfos)
    0
