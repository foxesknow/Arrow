namespace Arrow.FSharp.Threading

type SingledState =
| Signaled
| NotSignaled


[<AutoOpen>]
module Primitives =
    let WaitForever = System.Threading.Timeout.Infinite
    let WaitForeverTimespan = System.Threading.Timeout.InfiniteTimeSpan


module EventWaitHandle =
    let makeManualResetEvent initialState =
        match initialState with
        | Signaled -> new System.Threading.ManualResetEvent(true)
        | NotSignaled -> new System.Threading.ManualResetEvent(false)

    let makeAutoResetEvent initialState =
        match initialState with
        | Signaled -> new System.Threading.AutoResetEvent(true)
        | NotSignaled -> new System.Threading.AutoResetEvent(false)

    let set (handle : System.Threading.EventWaitHandle) =
        if handle.Set() then
            ()
        else
            failwith "failed to set handle"


module Mutex =
    let makeMutex =
        new System.Threading.Mutex()

    let release (mutex : System.Threading.Mutex) =
        mutex.ReleaseMutex();


module Semaphore =
    let makeSemaphore initialCount maximumCount = 
        new System.Threading.Semaphore(initialCount, maximumCount)

    let release (semaphore : System.Threading.Semaphore) =
        semaphore.Release() |> ignore

    let releaseCount (numberToRelease : int) (semaphore : System.Threading.Semaphore) =
        semaphore.Release(numberToRelease) |> ignore


module WaitHandle =
    let wait (handle : System.Threading.WaitHandle) =
        handle.WaitOne() |> ignore

    let waitFor (milliseconds : int) (handle : System.Threading.WaitHandle) =
        if handle.WaitOne(milliseconds) then
            Signaled
        else
            NotSignaled

    let waitForTimespan (timespan : System.TimeSpan) (handle : System.Threading.WaitHandle) =
        if handle.WaitOne(timespan) then
            Signaled
        else
            NotSignaled

    let waitAll (handles : System.Threading.WaitHandle array) =
        System.Threading.WaitHandle.WaitAll(handles) |> ignore

    let waitAllFor (milliseconds : int) (handles : System.Threading.WaitHandle array) =
        if System.Threading.WaitHandle.WaitAll(handles, milliseconds) then
            Signaled
        else
            NotSignaled

    let waitAllForTimespan (timespan : System.TimeSpan) (handles : System.Threading.WaitHandle array) =
        if System.Threading.WaitHandle.WaitAll(handles, timespan) then
            Signaled
        else
            NotSignaled

    let waitAny (handles : System.Threading.WaitHandle array) =
        let index = System.Threading.WaitHandle.WaitAny(handles)
        Some index

    let waitAnyFor (milliseconds : int) (handles : System.Threading.WaitHandle array) =
        let index = System.Threading.WaitHandle.WaitAny(handles, milliseconds)
        if index = System.Threading.WaitHandle.WaitTimeout then
            Some index
        else
            None

    let waitAnyForTimespan (timespan : System.TimeSpan) (handles : System.Threading.WaitHandle array) =
        let index = System.Threading.WaitHandle.WaitAny(handles, timespan)
        if index = System.Threading.WaitHandle.WaitTimeout then
            Some index
        else
            None


    



