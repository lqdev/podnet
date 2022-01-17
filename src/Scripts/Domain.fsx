module Domain

#r "nuget:FSharp.Data"

open System
open FSharp.Data

type Episode = {
    Title: string
    PubDate: DateTime
    EpisodeUrl: Uri
}

type FeedConfig = {
    url: string
    queue: int
}

type Feed = {
    Name: string
    Url: string
    Queue: int
    Content: HtmlDocument
}

type Config = {
    outputDirectory: string
    feeds: FeedConfig array
}