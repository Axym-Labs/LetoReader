# Axym-Reader
<div style="display: flex;">
  
![Website](https://img.shields.io/website?url=https%3A%2F%2Faxym.davidewiest.com) ![Docker Pulls](https://img.shields.io/docker/pulls/davidewiest/reader) [![.NET build](https://github.com/Axym-Labs/Axym-Reader/actions/workflows/dotnet-desktop.yml/badge.svg?branch=main)](https://github.com/Axym-Labs/Axym-Reader/actions/workflows/dotnet-desktop.yml) ![GitHub Issues or Pull Requests](https://img.shields.io/github/issues/axym-labs/Axym-Reader) [![License: GPL v3](https://img.shields.io/badge/License-GPLv3-blue.svg)](https://www.gnu.org/licenses/gpl-3.0)

</div>

- **[Live Demo](https://axym.davidewiest.com/read)**

<a href="https://hub.docker.com/r/davidewiest/reader">
  <img src="https://img.shields.io/badge/Docker-2CA5E0?style=for-the-badge&logo=docker&logoColor=white" />
</a>

### A highly customizable reader built as a direct alternative to paid speed-readers.
- Chunking, pacing and highlighting built in
  - great UX and responsive design
  - Easy importing
  - Very customizable (9 options and more to come)
  - Focus mode (big plus for dyslexia/add/adhd)
  - No external API dependencies
  - Local storage
  - Self-hostable

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
