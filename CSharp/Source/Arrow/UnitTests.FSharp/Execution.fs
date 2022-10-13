namespace Arrow.FSharp.Execution

open System;
open NUnit.Framework
open Arrow.FSharp.Execution

[<TestFixture>]
type DisposerTests() =
    [<Test>]
    member this.``Dispose called``() =
        let mutable called = false

        using(Disposer.makeDisposer (fun() -> called <- true)) (fun d ->
            called |> isFalse
        )

        called |> isTrue

    [<Test>]
    member this.``Only called once``() =
        let mutable count = 0

        count |> isEqualTo 0
        let d = Disposer.makeDisposer (fun() -> count <- count + 1)

        d.Dispose()
        count |> isEqualTo 1

        d.Dispose()
        count |> isEqualTo 1

         

