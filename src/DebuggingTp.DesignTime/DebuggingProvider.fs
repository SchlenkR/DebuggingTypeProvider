namespace DebuggingTp.DesignTime

open System.Reflection
open ProviderImplementation.ProvidedTypes
open FSharp.Core.CompilerServices
open DebuggingTp

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

    let providerName = "DebuggingTp"
    let providerNamespaceName = "DebuggingTp"
    let logfileParameterName = "Logfile"
    
    let asm = Assembly.GetExecutingAssembly()

    // check we contain a copy of runtime files, and are not referencing the runtime DLL
    do assert (typeof<TpRuntime>.Assembly.GetName().Name = asm.GetName().Name)

    let debuggingProvider =
        let providerType =
            ProvidedTypeDefinition(
                asm,
                providerNamespaceName,
                providerName,
                Some typeof<obj>,
                isErased = false
            )
        do providerType.DefineStaticParameters(
            [ProvidedStaticParameter(logfileParameterName, typeof<string>)],
            fun typeName args ->
                let logfile = unbox<string>(args.[0])
                do Shared.HostInfos.writeHostingInfos logfile
                let td = 
                    ProvidedTypeDefinition(
                        asm,
                        providerNamespaceName,
                        typeName, 
                        Some typeof<obj>, 
                        isErased = false)
                do
                    let p = 
                        ProvidedProperty(
                            "HostingInfos",
                            typeof<string>,
                            isStatic = true,
                            getterCode = (fun args ->
                                <@@ "Hosting infos (runtime): " + Shared.HostInfos.getHostingInfos () @@>))
                    p.AddXmlDoc($"Hosting infos (compile time) {Shared.HostInfos.getHostingInfos ()}")
                    td.AddMember p
                td
        )

        providerType

    do
        this.AddNamespace(providerNamespaceName, [debuggingProvider])
