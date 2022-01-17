# Podnet

F# console app for downloading and managing podcasts.

## Why?

TLDR: I want to leverage cross-platform frameworks like .NET to run anywhere and use open standards like [RSS](https://wikipedia.org/wiki/RSS) and [OPML](http://opml.org/) to improve interoperability and avoid lock-in.

My podcast download and management setup is more complex than I'd like it to be. I subscribe mainly using RSS feeds which I store and manage via [NewsBlur](https://www.newsblur.com/) alongside my other news feeds and blogs. NewsBlur is my source of Truth. When I want to listen to an episode, I can stream the mp3 audio file via VLC whether that's on Windows, Linux, Mac, or any other OS that supports streaming playback through VLC. The benefit of this solution is its flexibility. All I need is a browser to view and listen to the podcasts I'm subscribed to without being locked into any single podcast client. However, this solution also assumes I have internet connection, which is not always the case. Although I could save the mp3 file to listen later offline, it just saves to my Downloads folder and there's no organization. There's also no way to automate this process and say, "Download all the latest episodes for podcasts I'm subscribed to". 

To solve the download problem, I've been using [MusicBee](https://getmusicbee.com/) to subscribe to and download the latest episodes of my podcasts. Once my downloads are ready, I sync them to my [FiiO M6](https://www.fiio.com/m6) player. This solution has worked great for the past year. However, as far as I know, MusicBee is Windows only and there's no way to customize how many downloaded episodes to keep. Additionally, cleanup is something I do on a weekly basis using an [F# script](https://gist.github.com/lqdev/9c1eff502f725a24cb67d7fc7d7cddd9) I wrote previously. 

Because the process to transfer files to my MP3 player only involves copying the files to an SD card, I don't need anything fancy. That's how this app came about. For the time being, I can

- Keep track of all my feeds in a single place like NewsBlur through the config file.
- Specify how many episodes I want to keep per feed. 
- Automatic. 
- By generating an OPML file, I can quickly import all my subscriptions to any other client that supports it with ease and start listening there. 

The app is relatively minimal for now, but I'll keep improving it to make it a more long-term solution for my needs and hopefully it's useful for other as well. 

## Config

The configuration is relatively minimal. By default, there's a sample configuration in the `src` directory by the name *config.template.json*.

```json
{
    "outputDirectory": ".",
    "feeds": [
        {
            "url": "http://mp3s.nashownotes.com/pc20rss.xml",
            "queue": 1
        }
    ]
}
```

## Known issues

- Sometimes the connection times out. As a workaround, you can rerun the application. Episodes that were downloaded are skipped and only the remaining episodes will download. 

### Config Properties

- **outputDirectory**: Where to save 
- **feeds**:
  - **url**: URL for the podcast's RSS feed
  - **queue**: How many episodes to download at a time

## To-Dos

- [x] Split downloads into folders based on show
- [x] Export to OPML
- [x] Clean-up old episodes
- [ ] Support dynamic playback speed (i.e. save episodes at 1.5x playback speed
- [ ] Import OPML
- [ ] Dynamically generate config file
- [ ] Add download progress indicator
- [ ] Add metadata to files
- [ ] Allow command-line inputs for config
- [ ] Publish app as .NET tool
