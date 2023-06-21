open System
open System.Reflection
open System.Runtime.Versioning
open System.Diagnostics

let entryAsm = Assembly.GetEntryAssembly()

entryAsm
    .GetCustomAttribute<TargetFrameworkAttribute>()
    .FrameworkName

entryAsm.Location

Process.GetCurrentProcess()

typeof<string>.Assembly.Location
