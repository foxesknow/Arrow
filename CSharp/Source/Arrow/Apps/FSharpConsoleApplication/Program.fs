// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.

open Arrow.FSharp.Application
open Arrow.FSharp.Threading

type AppMain() =
    inherit InteractiveServiceMain()

    override this.Start(handle, args) =
        printfn "Hello, world"

    override this.Stop() =
        printfn "Stopping"


[<EntryPoint>]
let main argv = 
    ApplicationRunner.runAsService<AppMain> argv
    //printfn "%A" 0
    0 // return an integer exit code
