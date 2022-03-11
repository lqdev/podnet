module Domain

open System
open FSharp.Data
open System.Text.Json.Serialization
open Argu

type CliArguments = 
    | Config_File of path:string

    interface IArgParserTemplate with
        member s.Usage =
            match s with
            | Config_File _ -> "Specify a JSON config file"

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
    [<JsonPropertyName("feeds")>] Feeds: FeedConfig array
}