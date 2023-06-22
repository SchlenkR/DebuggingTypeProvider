module Program


let [<Literal>] LogFileName = "c:/temp/DebuggingTp.log"
type THostInfos = DebuggingTp.DebuggingTp<LogFileName>


let [<Literal>] Cs = 
    $"""
        Server=(localdb)\MSSQLLocalDB;
        Database=Demo;
        User Id=DebuggingTp;
        Password=DebuggingTp;
    """
let [<Literal>] Sql = 
    "select count(*) from DemoTable"
//type TSql = DebuggingTp.DebuggingSqlTp<Cs, Sql>
    
    
[<EntryPoint>]
let main _ =
    printfn "%s" (THostInfos.HostingInfos)
    //printfn "%s" (TSql.SqlResult)
    0
