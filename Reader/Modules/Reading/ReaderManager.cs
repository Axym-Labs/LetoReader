using Reader.Data.Product;
using Microsoft.JSInterop;
using System.Text;
using Newtonsoft.Json;
using Reader.Modules.Logging;
using Reader.Data.Reading;
using HtmlAgilityPack;
using System.Linq;

namespace Reader.Modules.Reading;

public class ReaderManager
{
    public List<string> TextPieces { get; private set; } = new();
    public bool ReadingStatus { get; private set; }
    public StateManager StateManager { get; set; }
    public ReaderConfig Config;
    private SiteInteraction SiteInteraction;

    // This is a binding to avoid unnecessarily long names
    private ReaderState State { get => StateManager.CurrentState; set => StateManager.CurrentState = value; }

    private CancellationTokenSource ReadingTaskTokenSource = new();

    public ReaderManager(StateManager stateManager, ReaderConfig config, SiteInteraction siteInteraction)
    {
        StateManager = stateManager;
        Config = config;
        SiteInteraction = siteInteraction;
        SetupTextPieces();
    }

    public void SetupTextPieces()
    {
        var unvalidatedTextPieces = TextHelper.SeparateText(StateManager.CurrentText);

        List<string> newTextPieces = new();
        foreach (var currentTextPiece in unvalidatedTextPieces)
        {
            var textPiece = currentTextPiece;
            while (textPiece.Length > Config.WordCharLimit)
            {
                newTextPieces.Add(textPiece[..Math.Min(Config.WordCharLimit, textPiece.Length - 1)]);
                textPiece = textPiece.Substring(Config.WordCharLimit);
            }
            newTextPieces.Add(textPiece);
        }

        TextPieces = newTextPieces;
        ClampPosition();
    }

    public void HandleStartStop()
    {
        if (!ReadingStatus)
        {
            StartReadingTask();
        }
        else
        {
            StopReadingTask();
        }
    }

    public async Task StartReadingTask()
    {
        await Log.Information("ReaderContext: StartReadingTask");
        if (ReadingStatus)
            return;

        ReadingStatus = true;
        // start task
        ReadingTaskTokenSource = new CancellationTokenSource();
        var readerTask = new Task(async () =>
        {
            await ReadingTask((double)60 / Config.ReadingSpeed, ReadingTaskTokenSource.Token);
        }, ReadingTaskTokenSource.Token);
        readerTask.Start();

    }

    public async Task StopReadingTask()
    {
        await Log.Information("ReaderContext: StartReadingTask");
        if (!ReadingStatus)
            return;

        ReadingStatus = false;
        // stop task, if the task started
        if (ReadingTaskTokenSource != null)
        {
            ReadingTaskTokenSource.Cancel();
            ReadingTaskTokenSource.Dispose();
        }

    }

    private async Task ReadingTask(double interval, CancellationToken ct)
    {
        while (true)
        {
            if (State.PositionInfo.Position >= TextPieces.Count - 1 || ct.IsCancellationRequested)
            {
                ReadingStatus = false;
                await SiteInteraction.HandleSiteStateChanged();
                break;
            }

            State.PositionInfo.Position++;
            State.LastRead = DateTime.Now;

            _ = Task.Run(() => StateManager.SaveStates());
            _ = Task.Run(() => SiteInteraction.HandleSiteStateChanged());
            await Task.Delay(TimeSpan.FromSeconds(interval));
        }
    }

    public void HandleNavBefore()
    {
        State.PositionInfo.Position -= Config.WordNavCount;
        State.PositionInfo.Position = Math.Max(0, State.PositionInfo.Position);
    }

    public void HandleNavNext()
    {
        State.PositionInfo.Position += Config.WordNavCount;
        State.PositionInfo.Position = Math.Min(TextPieces.Count - 1, State.PositionInfo.Position);
    }

    public void ClampPosition()
    {
        State.PositionInfo.Position = Math.Min(TextPieces.Count - 1, Math.Max(0, State.PositionInfo.Position));
    }

    public Tuple<string, string, string> GetCurrentTextPiece()
    {
        string word = TextPieces[State.PositionInfo.Position];

        string front = word.Substring(0, (word.Length + 1) / 2 - 1);
        string middle = word.Substring((word.Length + 1) / 2 - 1, 1);
        string back = word.Substring((word.Length + 1) / 2);

        return Tuple.Create(front, middle, back);
    }

    public string GetTextPiecesLookAhead()
    {
        StringBuilder result = new StringBuilder();
        int totalChars = 0;
        foreach (string word in TextPieces.Skip(State.PositionInfo.Position + 1))
        {
            if (totalChars + word.Length <= Config.PeripheralCharsCount)
            {
                result.Append(word).Append(" ");
                totalChars += word.Length + 1;
            }
            else
            {
                break;
            }
        }
        return result.ToString();
    }

    public string GetTextPiecesLookBehind()
    {
        SetupTextPieces();

        if (State.PositionInfo.Position == 0)
            return "";

        List<string> result = new();
        int charCount = 0;

        int i = (int)State.PositionInfo.Position - 1;

        while (i >= 0 && charCount + TextPieces[i].Length <= Config.PeripheralCharsCount)
        {
            result.Add(TextPieces[i]);
            charCount += TextPieces[i].Length + 1;
            i--;
        }

        result.Reverse();

        return String.Join(" ", result).Trim();
    }
}
