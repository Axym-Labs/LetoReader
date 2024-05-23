# Leto-Reader
<div style="display: flex;">
  
![Website](https://img.shields.io/website?url=https%3A%2F%2Fleto.axym.org) ![Docker Pulls](https://img.shields.io/docker/pulls/davidewiest/reader) [![.NET build](https://github.com/Axym-Labs/Axym-Reader/actions/workflows/dotnet-desktop.yml/badge.svg?branch=main)](https://github.com/Axym-Labs/Axym-Reader/actions/workflows/dotnet-desktop.yml) ![GitHub Issues or Pull Requests](https://img.shields.io/github/issues/axym-labs/Axym-Reader) [![License: GPL v3](https://img.shields.io/badge/License-GPLv3-blue.svg)](https://www.gnu.org/licenses/gpl-3.0)

</div>

![Leto Showcase](Showcase-min.gif)

<p align="center" style="align-items: center">
    <a href="https://leto.axym.org/read" target="_blank"><b>Live Demo</b></a> •
    <a href="https://github.com/Axym-Labs/Leto-Reader/wiki" target="_blank"><b>Documentation</b></a> •
    <a href="https://hub.docker.com/r/davidewiest/reader"><b>Docker image</b></a>
</p>

### A highly customizable reader built as a direct alternative to paid speed-readers.
- Chunking, pacing and highlighting built in
- Great UX and responsive design
- Easy importing
- Very customizable (9 options and more to come)
- Focus mode (big plus for dyslexia/add/adhd)
- No external API dependencies
- Local storage
- Self-hostable

#### Import options
- From a URL
- uploading a file (.pdf, .md, .txt, .html, .epub)
- Clipboard
- Request body of GET request

Due to personal circumstances, Leto is currently in a maintenance mode, meaning our main focus is on handling priority tasks related to the repository. New feature requests can still be made [here](https://reader.canny.io/), but may take a while to get implemented. Issues and bugs will still be addressed so make sure to ![report](https://github.com/Axym-Labs/Leto-Reader/issues) them.

## Hosting
#### With docker
- `docker pull davidewiest/reader:latest`
- `docker run -p 5001:8080 davidewiest/reader:latest` (ports in the form of -p <reachablePort>:8080, where reachablePort is the accessible one)

#### Github and `dotnet run`
- `git pull https://github.com/davidewiest/Reader.git`
- `cd Reader/Reader/`
- `nohup dotnet run --environment Production --urls=http://localhost:5001/ > /Logs/std.log 2> Logs/err.log &`

### Unique aspects
- Open source and free forever
- Modern, minimalistic UI
- Privacy first: All data is stored on your device only.
- Super simple to use - 14 first-level interaction fields (buttons + text fields) in total


## Changelogs
- [Version 2](https://github.com/Axym-Labs/Axym-Reader/wiki/Changelog-Version-2)
- [Version 2.1](https://github.com/Axym-Labs/Axym-Reader/wiki/Changelog-Version-2.1)
- [Version 3](https://github.com/Axym-Labs/Axym-Reader/wiki/Changelog-Version-3)


Made with ❤️ and .NET Blazor
