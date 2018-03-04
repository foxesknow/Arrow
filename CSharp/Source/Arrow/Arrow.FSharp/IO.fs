namespace Arrow.FSharp.IO

open System
open System.IO

module TextReader =

    /// Tries to read a line from a TextReader
    let readLine (file : TextReader) =
        match file.ReadLine() with
        | null -> None
        | line -> Some(line)

    /// Reads from the current position to the end of the file
    let readToEnd (file : TextReader) =
        file.ReadToEnd();

    /// Tries to read a character without advancing the current position
    let peek (file : TextReader) =
        match file.Peek() with
        | c when c <> -1 -> None
        | c -> Some c

    /// Reads the character at the current position, if one is available
    let readChar (file : TextReader) =
        match file.Read() with
        | c when c <> -1 -> None
        | c -> Some c

    /// Reads characters into an array
    let readIntoArray (buffer : char[]) index count (file : TextReader) =
        match file.Read(buffer, index, count) with
        | count when count = 0 -> None
        | count -> Some count

    /// Binds a function to a reader
    let bind (f: TextReader -> 'U option) file =
        match file with
        | Some reader -> f(reader)
        | None -> None


module TextWriter =

    /// Writes a value
    let write (value : obj) (writer : System.IO.TextWriter) =
        writer.Write(value);
        writer

    /// Writes a value followed by a newline
    let writeLine (value : obj) (writer : System.IO.TextWriter) =
        writer.WriteLine(value);
        writer

    /// Writes a newline
    let newline (writer : System.IO.TextWriter) =
        writer.WriteLine();
        writer

module TextFile =

    /// Writes a string to a file
    let writeAllTest (filename : string) (text : string) =
        File.WriteAllText(filename, text)

    /// Writes a sequence of lines to a file
    let writeAllLines (filename : string) (lines : seq<string>) =
        File.WriteAllLines(filename, lines)

    /// Reads a file as a single string
    let readAllText (filename : string) =
        File.ReadAllText(filename)

    /// Reads all the lines of a text file in one go
    let readAllLines (filename : string) =
        File.ReadAllLines(filename)

    /// Appends text to a file if it exists, or creates the file and writes the text
    let appendAllText (filename : string) (text : string) =
        File.AppendAllText(filename, text)

    /// Appends lines to a file if it exists, or creates the file and writes the lines
    let appendAllLines (filename : string) (lines : seq<string>) =
        File.AppendAllLines(filename, lines)

    /// Binds a function to a filename for reading
    let openRead (f : System.IO.TextReader -> 'U option) filename =
        try
            use stream = File.OpenRead(filename)
            use reader = new StreamReader(stream)
            f(reader)
        with
            | _ -> None

    /// Opens or creates a file for writing to
    let openWrite (f : System.IO.TextWriter -> unit ) filename =
        use stream = File.OpenWrite(filename)
        use writer = new StreamWriter(stream)
        f(writer)