namespace DebuggingTp.DesignTime

open System.IO
open System.Reflection
open ProviderImplementation.ProvidedTypes
open FSharp.Core.CompilerServices
open DebuggingTp

module Consts =
    let providerNamespace = "DebuggingTp"

type GenerativeProviderBase
    (
        config : TypeProviderConfig,
        providerNamespaceName: string,
        providerName: string,
        staticParams: list<ProvidedStaticParameter>,
        populate: obj array ->  ProvidedTypeDefinition -> unit
    ) as this 
    =
    inherit TypeProviderForNamespaces(
        config,
        assemblyReplacementMap = 
            [
                "DebuggingTp.DesignTime", "DebuggingTp"
            ],
        addDefaultProbingLocation = true
    )

    do
        this.RegisterProbingFolder (config.ResolutionFolder)

    let thisAssembly  = Assembly.GetExecutingAssembly()

    // check we contain a copy of runtime files, and are not referencing the runtime DLL
    do assert (typeof<TpRuntime>.Assembly.GetName().Name = thisAssembly.GetName().Name)

    do
        // TODO: always given?
        let basePath = Path.GetDirectoryName(typeof<TpRuntime>.Assembly.Location)
        this.RegisterProbingFolder(Path.Combine(basePath, @"Microsoft.Data.SqlClient-5.1.1\net461"))

    let providerType =
        ProvidedTypeDefinition(
            thisAssembly,
            providerNamespaceName,
            providerName,
            Some typeof<obj>,
            isErased = false
        )
    do providerType.DefineStaticParameters(
        staticParams,
        fun typeName args ->
            let asm = ProvidedAssembly()
            let td = 
                ProvidedTypeDefinition(
                    asm,
                    providerNamespaceName,
                    typeName, 
                    Some typeof<obj>, 
                    isErased = false)
            do populate args td
            do asm.AddTypes [ td ]
            td
        )
    do this.AddNamespace(providerNamespaceName, [providerType])

[<TypeProvider>]
type HostingInfosProviderImplementation(config : TypeProviderConfig) =
    inherit GenerativeProviderBase(
        config, 
        Consts.providerNamespace,
        "HostingInfosProvider",
        [ 
            ProvidedStaticParameter("Logfile", typeof<string>)
        ],
        fun args td ->
            //let logfile = unbox<string>(args.[0])
            //do Shared.HostInfos.writeHostingInfos logfile
            let p = 
                ProvidedProperty(
                    "HostingInfos",
                    typeof<string>,
                    isStatic = true,
                    getterCode = (fun args ->
                        <@@ "Hosting infos (runtime): " + Shared.HostInfos.getHostingInfos () @@>))
            p.AddXmlDoc($"Hosting infos (design time) {Shared.HostInfos.getHostingInfos ()} - config.ResolutionFolder = {config.ResolutionFolder} - tpRuntimeAsmLoc = {typeof<TpRuntime>.Assembly.Location}")
            td.AddMember p
    )

[<TypeProvider>]
type SqlClientProviderImplementation(config : TypeProviderConfig) =
    inherit GenerativeProviderBase(
        config,
        Consts.providerNamespace,
        "SqlClientProvider",
        [
            ProvidedStaticParameter("ConnectionString", typeof<string>);
            ProvidedStaticParameter("SelectScalarCommandText", typeof<string>)
        ],
        fun args  td ->
            let cs = unbox<string>(args.[0])
            let sql = unbox<string>(args.[1])
            let p = 
                ProvidedProperty(
                    "SqlResult",
                    typeof<string>,
                    isStatic = true,
                    getterCode = (fun args ->
                        <@@ "Sql result (runtime): " + Shared.Sql.executeScalar cs sql @@>))
            p.AddXmlDoc($"Sql result (design time): {Shared.Sql.executeScalar cs sql}")
            td.AddMember p
    )
