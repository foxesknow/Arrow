namespace UnitTests.FSharp.Storage

open System
open NUnit.Framework

open Arrow.FSharp.IO;
open Arrow.FSharp.Storage


[<TestFixture>]
type ResourceAccessorTests() =
    [<Test>]
    member this.Load() =
        let uri = makeUri "res://UnitTests.Arrow/UnitTests/Arrow/Resources/Jack.xml"
        let accessor = StorageManager.get uri

        use stream = Accessor.openRead accessor
        use reader = TextReader.fromStream stream
        TextReader.readToEnd reader |> isNotEmpty

