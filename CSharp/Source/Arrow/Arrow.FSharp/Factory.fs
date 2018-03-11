namespace Arrow.FSharp.Factory

module RegisteredTypeInstaller =
    /// Loads any types from the App.Config file and registers them
    let loadFromAppConfig() =
        Arrow.Factory.RegisteredTypeInstaller.LoadTypesFromAppConfig()

    /// Loads types from a particular assembly
    let loadFromAssembly(assembly : System.Reflection.Assembly) = 
        Arrow.Factory.RegisteredTypeInstaller.LoadTypes(assembly)

