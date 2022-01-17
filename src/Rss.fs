module Rss

open System
open System.IO
open System.Net.Http
open FSharp.Data
open Domain

let getFeed (feed : FeedConfig) = 

    let feedContents = HtmlDocument.Load(feed.url)

    let feedName = 
        feedContents.Descendants("channel")
        |> Seq.map(fun v -> v.Elements("title"))
        |> Seq.head
        |> Seq.head
        |> fun t -> t.InnerText()

    { Name = feedName; Content = feedContents; Queue = feed.queue; Url = feed.url}

let getInnerValue (node:HtmlNode) (el:string) = 
    node.Descendants(el)
    |> Seq.map(fun x -> x.InnerText())
    |> Seq.head

let getEpisodes (feed:Feed) = 
    feed.Content.Descendants("item")
    |> Seq.map(fun i -> 
        let title = getInnerValue i "title" 
        let pubDate = getInnerValue i "pubDate" |> DateTime.Parse

        let mediaLink = 
            i.Elements("enclosure")
            |> Seq.choose(fun x -> 
                x.TryGetAttribute("url") 
                |> Option.map(fun x -> x.Value()))
            |> Seq.head

        { Title=title; PubDate = pubDate; EpisodeUrl = new Uri(mediaLink)})
    |> Seq.sortByDescending(fun episode -> episode.PubDate)

let downloadEpisodeAsync (client:HttpClient) (filePath:string) (url:Uri) = 
    async {
        match File.Exists(filePath) with 
        | true -> ignore |> ignore
        | false ->
                        
            printfn $"Downloading {filePath}"

            let! audioStream = client.GetByteArrayAsync(url) |> Async.AwaitTask

            printfn $"{filePath} download complete"

            File.WriteAllBytesAsync(filePath, audioStream) |> Async.AwaitTask |> ignore
    }