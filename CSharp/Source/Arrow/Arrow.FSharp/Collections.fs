namespace Arrow.FSharp.Collections

open System.Collections.Generic 

module Memorization =
    /// Create s simple memorizer
    let memorize f =
        let cache = Dictionary<_, _>()
        fun x ->
            match cache.TryGetValue(x) with
            | (true, value) -> value
            | _ ->
                let value = f x
                cache.Add(x, value)
                value

    /// Create a recursive memorizer that can call back in to the memorization
    let recursiveMemorize f =
        let mem = Dictionary<_, _>();
        let rec recurse key =
            match mem.TryGetValue(key) with
            | (true, value) -> value
            | _ ->
                let value = f recurse key
                mem.Add(key, value)
                value
        recurse

