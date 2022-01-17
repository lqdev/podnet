open System
open System.Net.Http
open System.IO
open System.Text.Json
open Domain
open Rss
open Opml
open Utils
open FSharp.Data

let configFilePath = Path.Join(__SOURCE_DIRECTORY__, "config.template.json")

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
let client = new HttpClient()
client.Timeout <- TimeSpan.FromMinutes(10)


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
            
            // Delete old podcasts (if any)
            if files.Length > 0 then
                files 
                |> Seq.iter(fun x -> 
                    printfn $"Cleaning up {x.Name}"
                    x.Delete())

            // Define file name
            let extension = Path.GetExtension(ep.EpisodeUrl.AbsolutePath)
            let fileName = Path.Join(saveDirectory, $"{ep.Title}{extension}")

            // Download and save episode
            downloadEpisodeAsync client fileName ep.EpisodeUrl))
|> Async.Parallel
|> Async.Ignore
|> Async.RunSynchronously

