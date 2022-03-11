module Opml

open System.Linq
open System.Xml.Linq
open Domain

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

let fromOpml (path: string) = 
    failwith $"Implement OPML import"