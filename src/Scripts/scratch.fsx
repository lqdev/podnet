#load "/home/lqdev/Dev/podnet/src/Domain.fsx"
#load "/home/lqdev/Dev/podnet/src/Rss.fsx"

open System.IO
open System.Linq
open System.Xml.Linq
open System.Text.Json
open Domain
open Rss

let configFile = Path.Join(__SOURCE_DIRECTORY__, "config.json")

let configContent = 
    File.ReadAllText(configFile)
    |> fun x -> JsonSerializer.Deserialize<Config>(x)

let toOmpl (configFeed:Feed seq) = 

    let outlines = 
        configFeed
        |> Seq.map(fun x -> 
            XElement(XName.Get "outline", 
                XAttribute(XName.Get "Name", x.Name),
                XAttribute(XName.Get "xmlUrl", x.Url)))
    
    let feedOpml = 
        XElement(XName.Get "opml",
            XAttribute(XName.Get "version", "2.0"),
            XElement(XName.Get "head",
                XElement(XName.Get "title", "Podcast Subscriptions")),
            XElement(XName.Get "body"))

    feedOpml.Descendants(XName.Get "body").First().Add(outlines)

    feedOpml

configContent.feeds
|> Seq.map(fun x -> getFeed x)
|> toOmpl 
|> string 
|> fun x -> File.WriteAllText(Path.Join(__SOURCE_DIRECTORY__, "feeds.opml"),x)