// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.

open Arrow.FSharp.Collections

let fib r n =
  match n with
  | 0 -> 0
  | 1 -> 1
  | n -> r (n - 1) + r (n - 2)

[<EntryPoint>]
let main argv = 
    let f = Memorization.recursiveMemorize fib
    let result = f 10
    printfn "%A" result
    0 // return an integer exit code
