(* RSS service *)

module Rss

#load "/home/lqdev/Dev/podnet/src/Scripts/Domain.fsx"

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

let downloadEpisode (filePath:string) (url:Uri) = 
    async {
        match File.Exists(filePath) with 
        | true -> ignore |> ignore
        | false ->
            use client = new HttpClient()
            let! audioStream = client.GetStreamAsync(url) |> Async.AwaitTask

            let file = File.Create(filePath)

            audioStream.CopyToAsync(file) |> Async.AwaitTask |> ignore
            file.FlushAsync() |> Async.AwaitTask |> ignore
    }