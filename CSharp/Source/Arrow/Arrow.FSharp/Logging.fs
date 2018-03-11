namespace Arrow.FSharp.Logging

open FSharp.Core


type Log = {log : Arrow.Logging.ILog} with

    /// Writes to the debug log
    member this.debug format =
        Printf.ksprintf (fun text -> this.log.Debug(text)) format

    /// Writes to the info log
    member this.info format=
        Printf.kprintf (fun text -> this.log.Info(text)) format

    /// Writes to the warning log
    member this.warn format=
        Printf.kprintf (fun text -> this.log.Warn(text)) format

    /// Writes to the error log
    member this.error format=
        Printf.kprintf (fun text -> this.log.Error(text)) format

    /// Writes to the fatal log
    member this.fatal format=
        Printf.kprintf (fun text -> this.log.Fatal(text)) format


module LogManager =
    /// Returns the default log
    let getDefaultLog() =
        let log = Arrow.Logging.LogManager.GetDefaultLog()
        {Log.log = log}

    /// Returns the named log
    let getNamedLog (name : string) =
        let log = Arrow.Logging.LogManager.GetLog(name)
        {Log.log = log}

    /// Returns the log for a given type
    let getLogForType (type' : System.Type) =
        let log = Arrow.Logging.LogManager.GetLog(type')
        {Log.log = log}

    /// Returns the log for a given type
    let getGenericLog<'T> () =
        let log = Arrow.Logging.LogManager.GetLog<'T>()
        {Log.log = log}

    /// Attached an existing log to the F# logging framework
    let attach (log : Arrow.Logging.ILog) =
        {Log.log = log}
