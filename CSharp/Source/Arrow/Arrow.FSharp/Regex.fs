namespace Arrow.FSharp

open System.Text
open System.Text.RegularExpressions

module Regex =

    /// Try to match a string to a pattern
    let tryMatch (pattern : string) (text : string) =
        let m = Regex.Match(text, pattern)
        match m.Success with
        | false -> None
        | true -> Some(m.Groups |> Seq.cast<Group> |> Seq.toList)

    /// Checks if a string matches a pattern
    let isMatch (pattern : string) (text : string) =
        Regex.IsMatch(text, pattern)