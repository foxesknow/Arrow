// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.

open Arrow.FSharp.Attempt

let divide x y =
    match y with
    | 0 -> Failure "divide by zero"
    | _ -> Success (x / y)

[<EntryPoint>]
let main argv = 

    let x = attempt {
        try
            return 10 / 0
        with
        | e -> 
            return! Failure e.Message
    }

    match x with
    | Success s -> printfn "Success: %A" s
    | Failure reason -> printfn "Failure: %A" reason
    
    //printfn "%A" 0
    0 // return an integer exit code
