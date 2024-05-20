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
    public ReaderManager Manager { get; private set; }
    public ReaderConfigManager ConfigManager { get; private set; } = default!;
    public StateManager StateManager { get; private set; } = default!;

    public ReaderConfig Config { get; set; } = default!;

    public MudTextField<string> StateTitleField { get; set; } = new();
    public MudTextField<string> StateTextField { get; set; } = new();

    private SiteInteraction SiteInteraction { get; set; }

    private ILocalStorageService localStorage = default!;

    public ReaderContext(SiteInteraction siteInteraction, ILocalStorageService localStorage)
    {
        SiteInteraction = siteInteraction;
        this.localStorage = localStorage;
        StateManager = new StateManager(localStorage, siteInteraction, HandleSelectedReaderStateChanged);
    }

    public async Task TriggerOnInitializedEvents()
    {
        await SetConfig(ReaderConfig.GetDefault());

        

        // init reader
        InitializeReader();
        // init config manager
        InitializeConfigManager();
    }

    public async Task TriggerAfterFirstRenderEvents()
    {

        await StateManager.LoadSavedStates();

        if (StateManager.CurrentState == null)
            throw new("State must be initialized");
        if (Manager == null)
            throw new("Manager must be initialized");
        

        SiteInteraction.TriggerAfterRenderEvents();

        await LoadConfig();

        // start reader if demo
        if (StateManager.CurrentState.Title == ReaderState.GetDemo(ReaderStateSource.Program).Item1.Title)
            Manager.StartReadingTask();
    }

    private async Task LoadConfig()
    {
        ReaderConfig? loadedConfig = await ReaderConfig.LoadConfig(localStorage);
        if (loadedConfig != null)
        {
            Config = loadedConfig;
            InitializeConfigManager();

            // should work without this line
            await SetConfig(Config);
        }
    }

    public async Task SetConfig(ReaderConfig newConfig)
    {
        Config = newConfig;

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

        if (StateManager.CurrentState == null)
            throw new("State must be initialized");
        if (Config == null)
            throw new("Config must be initialized");

        Manager = new(StateManager, Config, SiteInteraction);
    }

    private void InitializeConfigManager()
    {
        if (Manager == null)
            throw new("Manager must be initialized");
        if (Config == null)
            throw new("Config must be initialized");

        ConfigManager = new(Config, SiteInteraction, Manager.SetupTextPieces, localStorage);
    }

    public async Task HandleNewText()
    {
        await HandleNewText(ReaderState.GetNew(ReaderStateSource.Internal, "Manual creation"));
    }

    private async Task HandleNewText(Tuple<ReaderState,string> stateAndText)
    {
        Log.Information("ReaderContext: HandleNewText");

        await StateManager.AddState(stateAndText.Item1, stateAndText.Item2);
        await StateManager.SaveStates();
    }

    public async Task HandlePasteTitle()
    {
        StateManager.CurrentState.Title = await SiteInteraction.JSRuntime.InvokeAsync<string>("getClipboardContent");

        await StateTitleField.SetText(StateManager.CurrentState.Title);
        await SiteInteraction.HandleSiteStateChanged();
    }

    public async Task HandlePasteText()
    {
        StateManager.CurrentText = await SiteInteraction.JSRuntime.InvokeAsync<string>("getClipboardContent");

        await StateTextField.SetText(StateManager.CurrentText);
        await SiteInteraction.HandleSiteStateChanged();
    }

    public async Task HandleTextChanged(string Text)
    {
        StateManager.CurrentText = Text;
        if (StateManager.CurrentText == string.Empty)
        {
            StateManager.CurrentText = ProductConstants.DefaultNewText;
        } else
        {
            Manager.SetupTextPieces();
        }

        await StateManager.SaveState(StateManager.CurrentState, StateManager.CurrentText);
    }

    public async Task HandleTitleChanged(string newTitle)
    {
        try
        {
            await StateManager.RenameState(StateManager.CurrentState, newTitle);
        } catch
        {
            Log.Error("ReaderContext: HandleTitleChanged: Could not rename state");   
        }

        await SiteInteraction.HandleSiteStateChanged();
    }



    public async Task OverwriteState()
    {
        PrepareSelectedStateChanging();
        await HandleSelectedReaderStateChanged();
    }


    public void PrepareSelectedStateChanging()
    {
        if (Manager != null)
            Manager.StopReadingTask();
    }

    public async Task HandleSelectedReaderStateChanged()
    {
        await SetStateFields();

        if (Manager != null)
            Manager.SetupTextPieces();
        
        if (Manager != null)
            Manager.ClampPosition();

        await SiteInteraction.HandleSiteStateChanged();
    }


    public async Task SetStateFields()
    {
        await StateTitleField.SetText(StateManager.CurrentState.Title);
        await StateTextField.SetText(StateManager.CurrentText);
    }
}
