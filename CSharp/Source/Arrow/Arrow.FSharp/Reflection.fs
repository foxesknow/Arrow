namespace Arrow.FSharp.Reflection

module TypeResolver =
    let coerceToType (type' : System.Type) (data : obj) =
        Arrow.Reflection.TypeResolver.CoerceToType(type', data)

    let expandEnum (type' : System.Type) (data : string) =
        Arrow.Reflection.TypeResolver.ExpandEnum(type', data)

    let expandedType (typeName : string) =
        Arrow.Reflection.TypeResolver.GetEncodedType(typeName)

    let loadAssembly (assemblyName : string) =
        Arrow.Reflection.TypeResolver.LoadAssembly(assemblyName)

    let tryLoadAssembly (assemblyName : string) =
        try
            let assembly = Arrow.Reflection.TypeResolver.LoadAssembly(assemblyName)
            Some assembly
        with
        | _ -> None


module Assembly =
    let getAssemblyForType<'T>() =
        let t = typedefof<'T>
        t.Assembly

