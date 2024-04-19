# Reader
- **[Demo Version](https://reader.davidewiest.com/)**
- **[Docker Image](https://hub.docker.com/r/davidewiest/reader)**

### A highly customizable reader built as a direct alternative to paid speed-readers.
- Speed-reading concepts such as chunking, pacing and highlighting are built into this reader.
- Start by reading the example text to get used to this kind of reading.
- Over time, increase reading speed.
### Functionality
- Use the focus mode to read.
- Import your texts either from your clipboard or by uploading a file (.pdf, .md, .txt, .html).
- Customize the reader in 9 different ways to fit your needs.

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
- Modern UI
- Privacy first: All data is stored on your device only.
- Super simple to use - 14 first-level interaction fields (buttons + text fields) in total
