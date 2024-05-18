namespace Reader.Modules.Reading;

using Blazored.LocalStorage;
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
using System.Collections.Generic;

public class ReaderContext
{
    public ReaderManager Manager { get; private set; } = default!;
    public ReaderConfigManager ConfigManager { get; private set; } = default!;
    public StateManager StateManager { get; private set; } = default!;
    private string CurrentStateTitle = default!;
    public ReaderState State => StateManager.ReaderStates[0];

    private ReaderConfig _config = default!;
    public ReaderConfig Config { get => _config; set => _config = value; }

    public MudTextField<string> StateTitleField { get; set; } = new();
    public MudTextField<string> StateTextField { get; set; } = new();

    private SiteInteraction SiteInteraction { get; set; }

    private bool skipNextStateUpdate = false;

    private ILocalStorageService localStorage = default!;

    public ReaderContext(SiteInteraction siteInteraction, ILocalStorageService localStorage)
    {
        SiteInteraction = siteInteraction;
        this.localStorage = localStorage;
        StateManager = new StateManager(localStorage);
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


        await StateManager.LoadSavedStates();

        await LoadConfig();

        // start reader if demo
        if (State.Title == ReaderState.GetDemo(ReaderStateSource.Program).Item1.Title)
            Manager.StartReadingTask();
    }

    private async Task LoadConfig()
    {
        ReaderConfig? loadedConfig = await localStorage.GetItemAsync<ReaderConfig>("readerState");
        if (loadedConfig != null)
        {
            _config = loadedConfig;
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

        ConfigManager = new(ref _config, SiteInteraction, Manager.SetupTextPieces, localStorage);
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
        StateManager.CurrentText = await SiteInteraction.JSRuntime.InvokeAsync<string>("getClipboardContent");

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
            try
            {
                await StateManager.RenameState(State, title);
            } catch
            {
                await Manager.UpdateSavedState();
                try
                {
                    await Manager.RenameSavedState(State.Title, title);
                } catch
                {
                    Log.Error("ReaderContext: HandleTitleChanged: Could not rename state");
                }
            }
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
        StateManager.CurrentState = newState;
        await HandleSelectedReaderStateChanged();
    }

    public async Task SwitchState(string title)
    {
        Log.Information("ReaderContext: SwitchState");

        if (Manager != null)
            Manager.StopReadingTask();

        CurrentStateTitle = title;
        await HandleSelectedReaderStateChanged();
    }

    public async Task AddAndSelectState(ReaderState newState)
    {
        newState.Text = TextHelper.Sanitize(newState.Text);
        PrepareSelectedStateChanging();
        await AddState(newState);
        CurrentStateTitle = newState.Title;
        await HandleSelectedReaderStateChanged();
    }

    public async Task AddState(ReaderState newState)
    {
        int i = 0;
        while (StateManager.ReaderStates(i == 0 ? newState.Title : newState.Title + $" ({i})"))
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

        if (Manager != null)
            Manager.StopReadingTask();

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



        var loadedStates =
            (await Task.WhenAll(
                (await localStorage.KeysAsync())
                .Where(key => key.StartsWith("TEXTSTATE:"))
                .Select(async key =>
                    ReaderState.ImportFromJson(
                        JsonConvert.DeserializeObject<JObject>(
                            (await localStorage.GetItemAsStringAsync(key))!
                        )!
                    )
                )
            ))
            .OrderByDescending(x => x.LastRead)
            .ToDictionary(x => x.Title, x => x);


        if (loadedStates == null)
            return;

        SavedStates = loadedStates;

        var currentState = ReaderState.Copy(State);


        SavedStates[currentState.Title] = currentState;

        // not required as currentstate set, but if that were to be removed
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
