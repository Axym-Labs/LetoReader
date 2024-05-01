namespace Reader.Modules.Reading;

using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using MudBlazor;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Reader.Data.Product;
using Reader.Data.Reading;
using Reader.Data.Storage;
using Reader.Modules.Logging;
using Reader.Modules.Product;

public class ReaderContext
{
    public ReaderManager Manager { get; private set; } = default!;
    public ReaderConfigManager ConfigManager { get; private set; } = default!;

    public Dictionary<string, ReaderState> SavedStates { get; private set; } = new();
    private string CurrentStateTitle = default!;

    public ReaderState State { get => SavedStates[CurrentStateTitle]; set => SavedStates[CurrentStateTitle] = value; }

    private ReaderConfig _config = default!;
    public ReaderConfig Config { get => _config; set => _config = value; }

    public MudTextField<string> StateTitleField { get; set; } = new();
    public MudTextField<string> StateTextField { get; set; } = new();

    private SiteInteraction SiteInteraction { get; set; }

    private bool skipNextStateUpdate = false;

    public ReaderContext(SiteInteraction siteInteraction)
    {
        SiteInteraction = siteInteraction;
    }

    public async Task TriggerOnInitializedEvents()
    {

        // set default state and config for initialization of reader
        await AddAndSelectState(ReaderState.GetDemo(ReaderStateSource.Program, "Automatic initialization until storage is loaded"));
        await SetConfig(ReaderConfig.GetDefault());

        // init reader
        InitializeReader();
        // init config manager
        InitializeConfigManager();
    }

    public async Task TriggerAfterFirstRenderEvents()
    {
        if (State == null)
            throw new("State must be initialized");
        if (Manager == null)
            throw new("Manager must be initialized");

        SiteInteraction.TriggerAfterRenderEvents();

        await LoadSavedStates();

        if (SavedStates.Count > 0)
        {
            // overwrite the default state with the first saved state
            await OverwriteState(SavedStates.First().Value);
        }

        await LoadConfig();

        // start reader if demo
        if (State.Title == ReaderState.GetDemo(ReaderStateSource.Program).Title)
            Manager.StartReadingTask();
    }

    private async Task LoadConfig()
    {
        string? loadedConfigStr = await SiteInteraction.JSRuntime.InvokeAsync<string?>("loadConfigurationStrIfExists");
        if (!string.IsNullOrEmpty(loadedConfigStr))
        {
            _config = JsonConvert.DeserializeObject<ReaderConfig>(loadedConfigStr!)!;
            InitializeConfigManager();

            // should work without this line
            await SetConfig(Config);
        }
    }

    public async Task SetConfig(ReaderConfig newConfig)
    {
        _config = newConfig;

        // should work without these 2 statements
        if (ConfigManager != null)
            ConfigManager.Config = Config;
        if (Manager != null)
            Manager.Config = Config;

        await SiteInteraction.HandleSiteStateChanged();
    }

    private void InitializeReader()
    {
        Log.Information("ReaderContext: InitializeReader");


        if (State == null)
            throw new("State must be initialized");
        if (Config == null)
            throw new("Config must be initialized");

        Manager = new(State, ref _config, SiteInteraction);
    }

    private void InitializeConfigManager()
    {
        if (Manager == null)
            throw new("Manager must be initialized");
        if (Config == null)
            throw new("Config must be initialized");

        ConfigManager = new(ref _config, SiteInteraction, Manager.SetupTextPieces);
    }

    public async Task HandleNewText()
    {
        await HandleNewText(ReaderState.GetNew(ReaderStateSource.Internal, "Manual creation"));
    }

    private async Task HandleNewText(ReaderState readerState)
    {
        Log.Information("ReaderContext: HandleNewText");

        skipNextStateUpdate = true;
        await AddAndSelectState(readerState);
        await Manager.UpdateSavedState();

    }

    public async Task HandlePasteTitle()
    {
        State.Title = await SiteInteraction.JSRuntime.InvokeAsync<string>("getClipboardContent");

        await OverwriteState(State);
        await StateTitleField.SetText(State.Title);
        await SiteInteraction.HandleSiteStateChanged();
    }

    public async Task HandlePasteText()
    {
        State.Text  = await SiteInteraction.JSRuntime.InvokeAsync<string>("getClipboardContent");

        await OverwriteState(State);
        await StateTextField.SetText(State.Text);
        await SiteInteraction.HandleSiteStateChanged();
    }

    public async Task HandleTextChanged(string Text)
    {
        State.Text = Text.Trim();
        if (State.Text == string.Empty)
        {
            State.Text = ProductConstants.DefaultNewText;

            // should work without this line
            await OverwriteState(State);
        } else
        {
            Manager.SetupTextPieces();
        }
        
        if (skipNextStateUpdate)
        {
            skipNextStateUpdate = false;
        } else
        {
            await Manager.UpdateSavedState();
        }
    }

    public async Task HandleTitleChanged(string title)
    {

        if (skipNextStateUpdate)
        {
            skipNextStateUpdate = false;
        }
        else
        {
            await Manager.RenameSavedState(State.Title, title);
        }

        State.Title = title.Trim();

        if (State.Title == string.Empty)
        {
            State.Title = ProductConstants.DefaultNewTitle;

            // should work without this line
            await OverwriteState(State);
        }

        await SiteInteraction.HandleSiteStateChanged();
    }



    // STATE MANAGER
    // this should be in its own class

    public async Task OverwriteState(ReaderState newState)
    {
        // must be at the start to prevent blocking of this update by the changes below, disabling the input field
        await SetStateFields(newState);
        PrepareSelectedStateChanging();
        State = newState;
        await HandleSelectedReaderStateChanged();
    }

    public async Task SwitchState(string title)
    {
        Log.Information("ReaderContext: SwitchState");

        CurrentStateTitle = title;
        await HandleSelectedReaderStateChanged();
    }

    public async Task AddAndSelectState(ReaderState newState)
    {
        PrepareSelectedStateChanging();
        await AddState(newState);
        CurrentStateTitle = newState.Title;
        await HandleSelectedReaderStateChanged();
    }

    public async Task AddState(ReaderState newState)
    {
        int i = 0;
        while (SavedStates.ContainsKey(newState.Title + $" ({i})"))
        {
            i++;
        }
        if (i > 0)
            newState.Title = newState.Title + $" ({i})";

        SavedStates[newState.Title] = newState;

        if (SiteInteraction.JsAllowed)
        {
            await SiteInteraction.JSRuntime.InvokeVoidAsync("setState", newState.Title, JsonConvert.SerializeObject(newState));
        }
        await SiteInteraction.HandleSiteStateChanged();
    }

    public void PrepareSelectedStateChanging()
    {
        if (Manager != null)
            Manager.StopReadingTask();
    }

    public async Task HandleSelectedReaderStateChanged()
    {
        await SetStateFields(State);

        // should work without this line
        if (Manager != null)
        {
            Manager.State = State;
            Manager.SetupTextPieces();
        }
        if (Manager != null)
            Manager.ClampPosition();

        await SiteInteraction.HandleSiteStateChanged();
    }

    public async Task LoadState(string title)
    {
        // fix this here
        Log.Information("ReaderPlatform: LoadTextState");

        CurrentStateTitle = title;

        await HandleSelectedReaderStateChanged();
        await SiteInteraction.HandleSiteStateChanged();

    }

    public async Task DeleteState(string title)
    {
        Log.Information("ReaderPlatform: DeleteTextState");

        Console.WriteLine(String.Join(", ", SavedStates.Keys));
        SavedStates.Remove(title);


        await SiteInteraction.JSRuntime.InvokeVoidAsync("deleteTextState", title);
        await LoadSavedStates();
        await SiteInteraction.HandleSiteStateChanged();
    }

    private async Task LoadSavedStates()
    {
        Log.Information("ReaderPlatform: ReloadSavedStates");

        var savedStatesStr = await SiteInteraction.JSRuntime.InvokeAsync<string>("loadStateArraysStr", null);

        if (JsonConvert.DeserializeObject<List<JObject>>(savedStatesStr) == null)
            return;

        var currentState = ReaderState.Copy(State);

        SavedStates =
            JsonConvert.DeserializeObject<List<JObject>>(savedStatesStr)!
            .Select(x => ReaderState.ImportFromJson(x))
            .OrderByDescending(x => x.LastRead)
            .ToList()
            .ToDictionary(x => x.Title, x => x);

        SavedStates[currentState.Title] = currentState;

        await HandlePossiblyNoState();
    }

    private async Task HandlePossiblyNoState()
    {
        if (SavedStates.Count < 0)
        {
            var newState = ReaderState.GetNew(ReaderStateSource.Internal, "Manual creation");
            CurrentStateTitle = newState.Title;
            SavedStates[newState.Title] = newState;
            await HandleSelectedReaderStateChanged();
        }
    }

    public async Task SetStateFields(ReaderState newState)
    {
        await StateTitleField.SetText(newState.Title);
        await StateTextField.SetText(newState.Text);
    }
}
