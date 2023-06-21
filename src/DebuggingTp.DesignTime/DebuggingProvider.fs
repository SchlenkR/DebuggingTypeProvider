namespace DebuggingTp.DesignTime

open System.Reflection
open ProviderImplementation.ProvidedTypes
open FSharp.Core.CompilerServices
open DebuggingTp

module Consts =
    let providerName = "DebuggingTp"
    let providerNamespaceName = "DebuggingTp"
    let logfileParameterName = "Logfile"

[<TypeProvider>]
type DebuggingProviderImplementation(config : TypeProviderConfig) as this =
    inherit TypeProviderForNamespaces(
        config,
        assemblyReplacementMap = 
            [
                "DebuggingTp.DesignTime", "DebuggingTp"
            ],
        addDefaultProbingLocation = true
    )
    
    let asm = Assembly.GetExecutingAssembly()

    // check we contain a copy of runtime files, and are not referencing the runtime DLL
    do assert (typeof<TpRuntime>.Assembly.GetName().Name = asm.GetName().Name)

    let debuggingProvider =
        let providerType =
            ProvidedTypeDefinition(
                asm,
                Consts.providerNamespaceName,
                Consts.providerName,
                Some typeof<obj>,
                isErased = false
            )
        do providerType.DefineStaticParameters(
            [ProvidedStaticParameter(Consts.logfileParameterName, typeof<string>)],
            fun typeName args ->
                let logfile = unbox<string>(args.[0])
                do Shared.writeHostingInfos logfile
                let td = 
                    ProvidedTypeDefinition(
                        asm,
                        Consts.providerNamespaceName,
                        typeName, 
                        Some typeof<obj>, 
                        isErased = false)
                let hostingInfosProp =
                    let p = 
                        ProvidedProperty(
                            "HostingInfos",
                            typeof<string>,
                            isStatic = true,
                            getterCode = (fun args ->
                                <@@ "Hosting infos (runtime): " + Shared.getHostingInfos () @@>))
                    p.AddXmlDoc($"Hosting infos (compile time):\n{Shared.getHostingInfos ()}")
                    p
                td.AddMembers(
                    [
                        ProvidedProperty(
                            "Logfile",
                            typeof<string>,
                            isStatic = true,
                            getterCode = (fun args -> <@@ logfile @@>))
                        hostingInfosProp
                    ]
                )
                td
        )

        providerType

    do
        this.AddNamespace(Consts.providerNamespaceName, [debuggingProvider])
