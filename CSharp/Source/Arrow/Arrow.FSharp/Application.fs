namespace Arrow.FSharp.Application

type InteractiveServiceMain = Arrow.Application.Service.InteractiveServiceMain
type ThreadedServiceMain = Arrow.Application.Service.ThreadedServiceMain

open Arrow.Application.Service

module ApplicationRunner =
    let runWithArgs (args : string[]) (action : string[] -> unit) =
        Arrow.Application.ApplicationRunner.Run(System.Action<_>(action), args)

    let run (action : unit -> unit) =
        let noArgs : string array = Array.zeroCreate 0
        let wrapper = fun args -> action()
        runWithArgs noArgs wrapper

    let runAndReturn (args : string[]) (action : string[] -> int) =
        Arrow.Application.ApplicationRunner.RunAndReturn(System.Func<_,_>(action), args)

    let runAsService<'T when 'T :> ServiceMain and 'T : (new : unit -> 'T)> (args : string[]) =
        let wrapper = fun args' ->
            let service = new InteractiveConsoleService<ServiceMainHost<'T>>();
            service.Run(args')

        runWithArgs args wrapper


