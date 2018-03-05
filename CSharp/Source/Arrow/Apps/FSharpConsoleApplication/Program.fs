// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.

open Arrow.FSharp.Logging

open FSharp.Core


let printIt format =
    Printf.kprintf(fun msg -> printfn "%s" msg) format
    

[<EntryPoint>]
let main argv = 
    let log = LogManager.getDefaultLog()
    log.debug "%A" 1

    printfn "%A" 0
    0 // return an integer exit code
