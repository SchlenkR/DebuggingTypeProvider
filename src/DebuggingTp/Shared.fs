module DebuggingTp.Shared

open System
open System.IO

module HostInfos =
    open System.Reflection
    open System.Runtime.Versioning
    open System.Text
    
    let getHostingInfos () =
        let sb = new StringBuilder()
        let print text = sb.AppendLine text |> ignore
        let printLoc ctx (asm: Assembly) =
            print $"""Location of '{ctx}' = {if asm = null then "null" else asm.Location}"""

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
        do File.AppendAllText(logfile, getHostingInfos())

module Sql =
    open Microsoft.Data
    open Microsoft.Data.SqlClient
    
    let executeScalar cs sql =
        try
            use conn = SqlConnection(cs)
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