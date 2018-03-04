namespace Arrow.FSharp.Execution

open System;
open NUnit.Framework
open Arrow.FSharp.Execution

module Tests =
    [<TestFixture>]
    type DisposerTests() =
        [<Test>]
        member this.``Dispose called``() =
            let mutable called = false

            using(Disposer.makeDisposer (fun() -> called <- true)) (fun d ->
                Assert.That(called, Is.False)
            )

            Assert.That(called, Is.True)

        [<Test>]
        member this.``Only called once``() =
            let mutable count = 0

            Assert.That(count, Is.EqualTo(0))
            let d = Disposer.makeDisposer (fun() -> count <- count + 1)

            d.Dispose()
            Assert.That(count, Is.EqualTo(1))

            d.Dispose()
            Assert.That(count, Is.EqualTo(1))        

         

