module Domain

open System
open FSharp.Data
open System.Text.Json.Serialization

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
    [<JsonPropertyName("outputDirectory")>] OutputDirectory: string
    [<JsonPropertyName("retentionPeriod")>] RetentionPeriod: int
    [<JsonPropertyName("feeds")>] Feeds: FeedConfig array
}