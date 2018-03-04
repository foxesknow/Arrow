namespace Arrow.FSharp

open System.Text
open System.Text.RegularExpressions

module String =

    /// Spit a string
    let split (separator : char) (text : string) =
        text.Split [| separator |] |> Array.toList 

    /// Join a sequence of strings
    let join (separator : string) (strings : string seq) =
        System.String.Join (separator, Seq.toArray strings)

    /// Determines if a string is null or empty
    let isNullOrEmpty text =
        System.String.IsNullOrEmpty text

    /// Determines if a string is null or whitespace
    let isNullOrWhiteSpace text =
        System.String.IsNullOrWhiteSpace text

    /// Converts a string to lower case
    let toLower (text : string) =
        text.ToLower()

    /// Converts a string to upper case
    let toUpper (text : string) =
        text.ToUpper()

    /// Trims excess whitespace
    let trim (text : string) =
        text.Trim()

