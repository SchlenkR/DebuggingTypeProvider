module DebuggingTp.Shared

open System
open System.IO
open Microsoft.FSharp.Core.CompilerServices

module HostInfos =
    open System.Reflection
    open System.Runtime.Versioning
    open System.Text
    
    let getHostingInfos () =
        let sb = new StringBuilder()
        let print text = sb.AppendLine text |> ignore
        let printKV k v = sb.AppendLine $"%s{k} = %s{v}" |> ignore
        let printLoc ctx (asm: Assembly) =
            printKV $"Location of '{ctx}'" (if asm = null then "null" else asm.Location)

        print $"----- {DateTime.Now}"
        let entryAsm = Assembly.GetEntryAssembly()

        try
            entryAsm
                .GetCustomAttribute<TargetFrameworkAttribute>()
                .FrameworkName
        with _ -> "-"
        |> fun x -> print $"EntryAsm TargetFrameworkAttribute: {x}"

        printLoc "EntryAsm" entryAsm
        printLoc "typeof<string>.Assembly" typeof<string>.Assembly

        print $"-----------------------"

        sb.ToString()

    let writeHostingInfos logfile =
        do File.AppendAllText(logfile, getHostingInfos ())

module Sql =
    open Microsoft.Data.SqlClient

    do AppContext.SetSwitch("Switch.Microsoft.Data.SqlClient.UseManagedNetworkingOnWindows", true)
    
    let executeScalar cs sql =
        try
            use conn = new SqlConnection(cs)
            use cmd =
                let c = conn.CreateCommand()
                do c.CommandText <- sql
                c
            conn.Open()
            let res = cmd.ExecuteScalar()
            conn.Close()
            string res
        with ex ->
            $"Error executing SQL: {ex}"