open System
open System.Net.Http
open System.IO
open System.Text.Json
open Domain
open Rss
open Opml
open FSharp.Data

let configFilePath = Path.Join(__SOURCE_DIRECTORY__, "config.json")

let config = 
    File.ReadAllText(configFilePath)
    |> fun x -> JsonSerializer.Deserialize<Config>(x)

// Create output directory if it doesn't exist
Directory.CreateDirectory(config.outputDirectory) |> ignore

// Feeds
let feeds = 
    config.feeds
    |> Seq.ofArray
    |> Seq.map(getFeed)

// Convert feeds to OPML
let opml = feeds |> toOmpl |> string

// Save opml
File.WriteAllText(Path.Join(config.outputDirectory,"feeds.opml"), opml)

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
        f.Episodes 
        |> Seq.map(fun ep ->
            let saveDirectory = Path.Join(config.outputDirectory,f.FeedName)
            
            Directory.CreateDirectory(saveDirectory) |> ignore
            
            // let fileName = Path.Join(saveDirectory, Path.GetFileName(ep.EpisodeUrl.AbsolutePath))
            let extension = Path.GetExtension(ep.EpisodeUrl.AbsolutePath)
            let fileName = Path.Join(saveDirectory, $"{ep.Title}{extension}")

            downloadEpisodeAsync client fileName ep.EpisodeUrl))
|> Async.Parallel
|> Async.Ignore
|> Async.RunSynchronously