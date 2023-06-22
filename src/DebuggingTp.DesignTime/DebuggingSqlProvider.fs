namespace DebuggingTp.DesignTime

open System.Reflection
open ProviderImplementation.ProvidedTypes
open FSharp.Core.CompilerServices
open DebuggingTp

[<TypeProvider>]
type DebuggingSqlProviderImplementation(config : TypeProviderConfig) as this =
    inherit TypeProviderForNamespaces(
        config,
        assemblyReplacementMap = 
            [
                "DebuggingTp.DesignTime", "DebuggingTp"
            ],
        addDefaultProbingLocation = true
    )
    
    let providerName = "DebuggingSqlTp"
    let providerNamespaceName = "DebuggingTp"
    let connectionStringParameterName = "ConnectionString"
    let selectScalarCommandTextParameterName = "SelectScalarCommandText"

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
            [
                ProvidedStaticParameter(connectionStringParameterName, typeof<string>);
                ProvidedStaticParameter(selectScalarCommandTextParameterName, typeof<string>)
            ],
            fun typeName args ->
                let cs = unbox<string>(args.[0])
                let sql = unbox<string>(args.[1])
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
                            "SqlResult",
                            typeof<string>,
                            isStatic = true,
                            getterCode = (fun args ->
                                <@@ "Sql result (runtime): " + Shared.Sql.executeScalar cs sql @@>))
                    p.AddXmlDoc($"Sql result (compile time): {Shared.Sql.executeScalar cs sql}")
                    td.AddMember p
                td
        )

        providerType

    do
        this.AddNamespace(providerNamespaceName, [debuggingProvider])
