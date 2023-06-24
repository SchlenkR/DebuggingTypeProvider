module Program

open System

module HostingInfos =
    let [<Literal>] LogFileName = "c:/temp/DebuggingTp.log"
    type THostInfos = DebuggingTp.HostingInfosProvider<LogFileName>

    // Have a look at the XML docs on `THostInfos.HostingInfos`
    let actualInfos = THostInfos.HostingInfos

module SQL =
    let [<Literal>] Cs = 
        $"""
            Server=(localdb)\MSSQLLocalDB;
            Database=Demo;
            User Id=DebuggingTp;
            Password=DebuggingTp;
        """
    let [<Literal>] Sql = 
        "select count(*) from DemoTable"
    type TSql = DebuggingTp.SqlClientProvider<Cs, Sql>

    // Have a look at the XML docs on `TSql.xx`
    //let actualInfos = TSql.R

    
[<EntryPoint>]
let main _ =
    printfn "Hosting infos: %s" HostingInfos.actualInfos
    //printfn "%s" (TSql.SqlResult)

    Console.ReadLine() |> ignore
    0
