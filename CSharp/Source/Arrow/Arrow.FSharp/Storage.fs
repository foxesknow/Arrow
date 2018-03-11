namespace Arrow.FSharp.Storage

[<AutoOpen>]
module Operators =
    let makeUri (location : string) =
        new System.Uri(location)

module StorageManager =
    /// Loads and registers and accessor from the app.config file
    let loadFromAppConfig() =
        Arrow.Storage.StorageManager.LoadFromAppConfig()

    /// Gets the accessor for the given uri
    let get (uri : System.Uri) =
        Arrow.Storage.StorageManager.Get(uri)

    /// Tries to get the access for the given uri
    let tryGet (uri : System.Uri) =
        match Arrow.Storage.StorageManager.TryGetAcccessor(uri) with
        | (true, accessor) -> Some accessor
        | (false, _) -> None

    /// Adds a new accessor
    let add (name : string) (accessor : System.Uri -> Arrow.Storage.Accessor) =
        Arrow.Storage.StorageManager.Add(name, System.Func<_,_>(accessor))

module Accessor =
    let openRead (accessor : Arrow.Storage.Accessor) =
        accessor.OpenRead()

    let openWrite (accessor : Arrow.Storage.Accessor) =
        accessor.OpenWrite()

    let uri (accessor : Arrow.Storage.Accessor) =
        accessor.Uri

    let canCallExists (accessor : Arrow.Storage.Accessor) =
        accessor.CanExists

    let exists (accessor : Arrow.Storage.Accessor) =
        accessor.Exists()