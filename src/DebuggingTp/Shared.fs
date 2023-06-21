module DebuggingTp.Shared

open System
open System.IO
open System.Reflection
open System.Runtime.Versioning
open System.Text

let getHostingInfos () =
    let sb = new StringBuilder()
    let print text = sb.AppendLine text |> ignore

    let printLoc ctx (asm: Assembly) =
        print $"""Location of '{ctx}' = {if asm = null then "null" else asm.Location}"""

    let entryAsm = Assembly.GetEntryAssembly()

    try
        entryAsm
            .GetCustomAttribute<TargetFrameworkAttribute>()
            .FrameworkName
    with _ -> "-"
    |> fun x -> print $"EntryAsm TargetFrameworkAttribute: {x}"

    printLoc "EntryAsm" entryAsm
    printLoc "typeof<string>.Assembly" typeof<string>.Assembly

    sb.ToString()

let writeHostingInfos logfile =
    do File.WriteAllText(logfile, getHostingInfos ())
