using Reader.Data.Product;
using Microsoft.JSInterop;
using System.Text;
using Newtonsoft.Json;
using Reader.Data.Storage;

namespace Reader.Modules;

public class ReaderManager
{
    public List<string> TextPieces { get; private set; } = new();
    public bool ReadingStatus { get; private set; }
    public ReaderState State;
    public ReaderConfigManager ConfigManager;

    public bool jsInteropAllowed = false;
    public bool ignoreNextTextChange = false;
    public bool ignoreNextTitleChange = false;

    private CancellationTokenSource ReadingTaskTokenSource = new();
    private Func<Action, Task> InvokeAsync;
    private Action StateHasChanged;
    private IJSRuntime JSRuntime;

    public ReaderManager(ref ReaderState state, Func<Action, Task> invokeAsync, Action stateHasChanged, IJSRuntime JSRuntime) {
        State = state;
        ConfigManager = new(JSRuntime, SetupTextPieces);
        InvokeAsync = invokeAsync;
        StateHasChanged = stateHasChanged;
        this.JSRuntime = JSRuntime;
        SetupTextPieces();
    }

    public void HandleStartStop() {
        if (!ReadingStatus) {
            StartReadingTask();
        } else { 
            StopReadingTask();               
        }
    }

    public void StartReadingTask() {
        ReadingStatus = true;
        // start task
        ReadingTaskTokenSource = new CancellationTokenSource();
        var readerTask = new Task(async () => {
            await ReadingTask((double)60/ConfigManager.Config.ReadingSpeed, ReadingTaskTokenSource.Token);
        }, ReadingTaskTokenSource.Token);
        readerTask.Start();
    }

    public void StopReadingTask() {
        ReadingStatus = false;
        // stop task
        if (ReadingTaskTokenSource != null) {
            ReadingTaskTokenSource.Cancel();
            ReadingTaskTokenSource.Dispose();
        }
    }

    private async Task ReadingTask(double interval, CancellationToken ct) {
        while (true)
        {
            if (State.Position>=TextPieces.Count-1 || ct.IsCancellationRequested) {
                ReadingStatus = false;
                await InvokeAsync(() => {
                    StateHasChanged();
                });
                break;
            }

            State.Position++;
            State.LastRead = DateTime.Now;
            // await not needed - waiting time would be less accurate
            UpdateSavedState();
            await InvokeAsync(() => {
                StateHasChanged();
            });
            await Task.Delay(TimeSpan.FromSeconds(interval));
        }
    }

    public void HandleNavBefore() {
        State.Position -= ConfigManager.Config.WordNavCount;
        State.Position = Math.Max(0, State.Position);
    }

    public void HandleNavNext() {
        State.Position += ConfigManager.Config.WordNavCount;
        State.Position = Math.Min(TextPieces.Count-1, State.Position);
    }

    public void SetupTextPieces() {
        var unvalidatedTextPieces = State.Text.Split(new string[] { " ", Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();

        List<string> newTextPieces = new();
        foreach (var currentTextPiece in unvalidatedTextPieces) {
            var textPiece = currentTextPiece; 
            while (textPiece.Length > ConfigManager.Config.WordCharLimit) {
                newTextPieces.Add(textPiece[..Math.Min(ConfigManager.Config.WordCharLimit, textPiece.Length-1)]);
                textPiece = textPiece.Substring(ConfigManager.Config.WordCharLimit);
            }
            newTextPieces.Add(textPiece);
        }

        TextPieces = newTextPieces;
        ClampPosition();
    }

    public void ClampPosition() {
        State.Position = Math.Min(TextPieces.Count-1, Math.Max(0, State.Position));
    }

    public async Task HandleTextChanged(string Text) {
        if (ignoreNextTextChange) {
            ignoreNextTextChange = false;
            return;
        }
        
        State.Text = Text.Trim();
        if (State.Text == string.Empty) {
            State.Text = ProductStorage.DefaultNewText;
        }
        SetupTextPieces();
        State.Position = 0;
        UpdateSavedState();
    }

    public async Task HandleTitleChanged(string title) {
        if (ignoreNextTitleChange) {
            ignoreNextTitleChange = false;
            return;
        }
        
        // if (jsInteropAllowed) {
        //     title = await JSRuntime.InvokeAsync<string>("getUniqueTitle", title);
        // }
        // Console.WriteLine(title);
        RenameSavedState(State.Title, title);
        State.Title = title;
    }

    public Tuple<string, string, string> GetCurrentTextPiece() {
        string word = TextPieces[State.Position];

        string front = word.Substring(0, (word.Length + 1) / 2 -1);
        string middle = word.Substring((word.Length + 1) / 2 - 1, 1);
        string back = word.Substring((word.Length + 1) / 2);

        return Tuple.Create(front, middle, back);
    }

    public string GetTextPiecesLookAhead() {
        StringBuilder result = new StringBuilder();
        int totalChars = 0;
        foreach (string word in TextPieces.Skip(State.Position + 1)) {
            if (totalChars + word.Length <= ConfigManager.Config.PeripheralCharsCount) {
                result.Append(word).Append(" ");
                totalChars += word.Length+1;
            } else {
                break;
            }
        }
        return result.ToString();
    }

    public string GetTextPiecesLookBehind() {
        StringBuilder result = new StringBuilder();
        int charCount = 0;

        int i = State.Position-1;
        while (i>=0 && charCount + TextPieces[i].Length <= ConfigManager.Config.PeripheralCharsCount)
        {
            result.Append(TextPieces[i]).Append(" ");
            charCount += TextPieces[i].Length+1;
            i--;
        }
        
        return result.ToString().Trim();
    }

    public async Task UpdateSavedState() {
        await JSRuntime.InvokeVoidAsync("updateState", State.Title, JsonConvert.SerializeObject(State));
    }

    private async Task RenameSavedState(string oldTitle, string newTitle) {
        await JSRuntime.InvokeVoidAsync("renameState", oldTitle, newTitle);
    }
}
