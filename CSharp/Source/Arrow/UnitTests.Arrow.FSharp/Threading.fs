namespace UnitTests.FSharp.Threading

open NUnit.Framework
open Arrow.FSharp.Threading;

[<TestFixture>]
type EventWaitHandleTests() =
    [<Test>]
    member this.``Create a manual reset event that is not signalled``() =
        use event = EventWaitHandle.makeManualResetEvent NotSignaled
        event |> isNotNull

    [<Test>]
    member this.``Create a manual reset event that is signalled``() =
        use event = EventWaitHandle.makeManualResetEvent Signaled
        event |> isNotNull

    [<Test>]
    member this.``Create an auto reset event that is not signalled``() =
        use event = EventWaitHandle.makeAutoResetEvent NotSignaled
        event |> isNotNull

    [<Test>]
    member this.``Create a auto reset event that is signalled``() =
        use event = EventWaitHandle.makeAutoResetEvent NotSignaled
        event |> isNotNull

    [<Test>]
    member this.``Set and wait for``() =
        use event = EventWaitHandle.makeManualResetEvent NotSignaled
        event |> EventWaitHandle.set
        event |> WaitHandle.waitFor WaitForever |> isEqualTo Signaled

    [<Test>]
    member this.``Wait for when not signaled``() =
        let event = EventWaitHandle.makeManualResetEvent NotSignaled
        event |> WaitHandle.waitFor 100 |> isEqualTo NotSignaled

    [<Test>]
    member this.``Run on thread``() =
        use beginProcessing = EventWaitHandle.makeManualResetEvent NotSignaled
        use doneProcessing = EventWaitHandle.makeManualResetEvent NotSignaled
        let mutable allDone = false
        
        let task = async {
            WaitHandle.wait beginProcessing
            allDone <- true
            EventWaitHandle.set doneProcessing
        }

        Async.Start task
        EventWaitHandle.set beginProcessing
        WaitHandle.wait doneProcessing
        allDone |> isTrue


