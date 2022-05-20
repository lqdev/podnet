module Utils

open System
open System.IO

// Get directories
let getDirectories topDir =
    let dir = DirectoryInfo(topDir)
    dir.GetDirectories()

// Operation to delete files
let deleteFiles (dir:DirectoryInfo) = 
    let files = 
        dir.GetFiles()
        |> Array.sortByDescending(fun file -> file.LastWriteTime)

    // Make sure that there are more than 1 episodes
    if files.Length > 1 then
        files
        |> Array.tail
        |> Array.iter(fun file -> file.Delete())

let sanitizeString (s:string) = 
    let invalidChars = 
        [|'!';':';' ';';';'#';''';'-';'/';',';'(';')';'|';'?'; '.'|]
        |> Array.append(Path.GetInvalidFileNameChars())
        |> Array.append (Path.GetInvalidPathChars())
    
    String.Join('_',s.Split(invalidChars, StringSplitOptions.RemoveEmptyEntries))