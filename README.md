# Axym-Reader
<div style="display: flex;">
<a href="https://axym.davidewiest.com">
  <img alt="Website" src="https://img.shields.io/website?url=https%3A%2F%2Faxym.davidewiest.com">
</a>  
![Docker Pulls](https://img.shields.io/docker/pulls/davidewiest/reader) [![.NET build](https://github.com/Axym-Labs/Axym-Reader/actions/workflows/dotnet-desktop.yml/badge.svg?branch=main)](https://github.com/Axym-Labs/Axym-Reader/actions/workflows/dotnet-desktop.yml) ![GitHub Issues or Pull Requests](https://img.shields.io/github/issues/axym-labs/Axym-Reader) [![License: GPL v3](https://img.shields.io/badge/License-GPLv3-blue.svg)](https://www.gnu.org/licenses/gpl-3.0)

</div>

- **[Website](https://axym.davidewiest.com/)**

<a href="https://hub.docker.com/r/davidewiest/reader">
  <img src="https://img.shields.io/badge/Docker-2CA5E0?style=for-the-badge&logo=docker&logoColor=white" />
</a>


### A highly customizable reader built as a direct alternative to paid speed-readers.
- Speed-reading concepts such as chunking, pacing and highlighting are built into this reader.
- Start by reading the example text to get used to this kind of reading.
- Over time, increase reading speed.

### Functionality
- Use the focus mode to read.
- Customize the reader in 9 different ways to fit your needs.

##### Import options
- From a URL
- uploading a file (.pdf, .md, .txt, .html, .epub)
- Clipboard
- Request body of GET request

### Hosting
##### With docker
- `docker pull davidewiest/reader:latest`
- `docker run -p 5001:8080 davidewiest/reader:latest` (ports in the form of -p <reachablePort>:8080, where reachablePort is the accessible one)

##### Github and `dotnet run`
- `git pull https://github.com/davidewiest/Reader.git`
- `cd Reader/Reader/`
- `nohup dotnet run --environment Production --urls=http://localhost:5001/ > /Logs/std.log 2> Logs/err.log &`

### Unique aspects
- Open source and free forever
- Modern, minimalistic UI
- Privacy first: All data is stored on your device only.
- Super simple to use - 14 first-level interaction fields (buttons + text fields) in total


### Changelogs
- [Version 2](https://github.com/Axym-Labs/Axym-Reader/wiki/Changelog-Version-2)


Made with <img src="https://img.shields.io/badge/.NET-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" />
