namespace Arrow.FSharp.Attempt

[<AutoOpen>]
module AttemptImpl =

    type Attempt<'T> =
        | Success of 'T
        | Failure of string


module Attempt =    
    type AttemptBuilder() =
        member this.Bind(r, f) =
            match r with
            | Success s -> 
                try
                    f s
                with
                    | e -> Failure e.Message
            | fail -> fail

        member this.Return(x) =
            Success x

        member this.ReturnFrom(r) =
            r

        (*
        member this.Yield(x) =
            Success x

        member this.YieldFrom(r) =
            r
        *)

        member this.Zero() = 
            Success ()

        member this.Combine(r1, f) =
            match r1 with
            | Success _ -> r1
            | Failure _ -> f()

        member this.Delay(f) =
            f

        member this.Run(f) =
            f()

        member this.While(guard, body) =
            if guard() then
                //body() |> ignore
                this.Bind(body(), fun() -> this.While(guard, body))
            else
                this.Zero()

        member this.TryWith(body, handler) =
            try
                this.ReturnFrom(body())
            with
                e -> handler e

        member this.TryFinally(body, compensation) =
            try
                this.ReturnFrom(body())
            finally
                compensation()

        member this.Using(disposable : #System.IDisposable, body) =
            try
                this.ReturnFrom(body(disposable))
            finally
                match disposable with
                | null -> ()
                | d -> d.Dispose()

        member this.For(sequence : seq<_>, body) =
            this.Using(sequence.GetEnumerator(), fun enum ->
                this.While(enum.MoveNext, this.Delay(fun() -> body enum.Current)))

    let execute f =
        try
            Success (f())
        with
        | e -> Failure e.Message

[<AutoOpen>]
module Implementation =
    let attempt = new Attempt.AttemptBuilder()


