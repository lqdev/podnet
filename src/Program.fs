open System
open System.Net.Http
open System.IO
open System.Text.Json
open Domain
open Rss
open Opml
open Utils
open Argu
open FSharp.Data

[<EntryPoint>]
let main args = 

    let parser = ArgumentParser.Create<CliArguments>(programName = "podnet")

    let parserResults = parser.Parse args

    let configFile = parserResults.GetResult Config_File

    let configFilePath = Path.Join(__SOURCE_DIRECTORY__, configFile)

    let config = 
        File.ReadAllText(configFilePath)
        |> fun x -> JsonSerializer.Deserialize<Config>(x)

    // Create output directory if it doesn't exist
    Directory.CreateDirectory(config.OutputDirectory) |> ignore

    // Feeds
    let feeds = 
        config.Feeds
        |> Seq.ofArray
        |> Seq.map(getFeed)

    // Convert feeds to OPML
    let opml = feeds |> toOmpl |> string

    // Save opml
    File.WriteAllText(Path.Join(config.OutputDirectory,"feeds.opml"), opml)

    // Get all episodes
    let episodes = 
        feeds
        |> Seq.map(fun x -> 

            let episodes =
                x |> getEpisodes |> Seq.take x.Queue 
            
            {|
                FeedName= x.Name
                Episodes = episodes |})     

    // Download episodes

    episodes
    |> Seq.collect(
        fun f -> 
            let feedEpisodes = f.Episodes  
            feedEpisodes
            |> Seq.map(fun ep ->
                // Define directory to save podcats for a specific feed
                let saveDirectory = Path.Join(config.OutputDirectory, f.FeedName)

                // Create directory to save podcats to if it doesn't exist           
                let di = Directory.CreateDirectory(saveDirectory)
                
                // Get old podcasts



                let files = 
                    di.GetFiles()
                    |> Seq.filter(fun x -> 
                        let filename = Path.GetFileNameWithoutExtension(x.FullName)
                        let episodesNames = feedEpisodes |> Seq.map(fun c -> c.Title)
                        episodesNames |> Seq.contains(filename)  |> not)
                    |> Array.ofSeq

                let numberOfEpisodes = feedEpisodes |> Seq.length

                // Delete old podcasts (if any)
                if files.Length > numberOfEpisodes then
                    files 
                    |> Seq.iter(fun x -> 
                        printfn $"Cleaning up {x.Name}"
                        x.Delete())

                // Define file name
                let extension = Path.GetExtension(ep.EpisodeUrl.AbsolutePath)
                let invalidChars = Path.GetInvalidFileNameChars()
                let sanitizedName = String.Join("_", ep.Title.Split(invalidChars, StringSplitOptions.RemoveEmptyEntries))

                let fileName = Path.Join(saveDirectory, $"{sanitizedName}{extension}")

                // Download and save episode
                downloadEpisodeAsync fileName ep.EpisodeUrl))
    |> Async.Sequential
    |> Async.Ignore
    |> Async.RunSynchronously

    0