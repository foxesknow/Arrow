namespace Arrow.FSharp.Execution

module Disposer =
    /// Create an IDisposable implementation that will call f once when disposing
    let makeDisposer f =        
        let mutable disposed = false
        {            
            new System.IDisposable with            
                member this.Dispose() =
                    match disposed with
                    | true -> ()
                    | false -> 
                            do f()
                            disposed <- true
        }

module MethodCall =
    /// Executes a function and ignores any exceptions it throws
    let allowFail f =
        try
            do f()
        with
            _ -> ()