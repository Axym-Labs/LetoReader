namespace Reader.Modules.Reading;

using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using MudBlazor;
using Newtonsoft.Json;
using Reader.Data.Product;
using Reader.Data.Reading;
using Reader.Data.Storage;
using Reader.Modules.Logging;
using Reader.Modules.Product;

public class ReaderContext
{
    public ReaderManager Manager { get; private set; } = default!;
    public ReaderConfigManager ConfigManager { get; private set; } = default!;

    private ReaderState _state = default!;
    public ReaderState State { get => _state; set => _state = value; }

    private ReaderConfig _config = default!;
    public ReaderConfig Config { get => _config; set => _config = value; }

    public MudTextField<string> stateTitle { get; set; } = new();
    public MudTextField<string> stateText { get; set; } = new();

    private SiteInteraction SiteInteraction { get; set; }

    private bool skipNextStateUpdate = false;

    public ReaderContext(SiteInteraction siteInteraction)
    {
        SiteInteraction = siteInteraction;
    }

    public async Task TriggerOnInitializedEvents()
    {
        // set default state and config for initialization of reader
        await SetState(ReaderState.GetDemo(ReaderStateSource.Program, "Automatic initialization until storage is loaded"));
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

        Manager.jsInteropAllowed = true;

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

    public async Task SetState(ReaderState newState)
    {
        // continue with a copy to ensure the newState is not overwritten somewhere else
        await HandleStateUpdated(ReaderState.Copy(newState));
        await SiteInteraction.HandleSiteStateChanged();
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
        if (State == null)
            throw new("State must be initialized");
        if (Config == null)
            throw new("Config must be initialized");

        Manager = new(ref _state, ref _config, SiteInteraction);

        Log.Information("ReaderContext: InitializeReader");
    }

    private void InitializeConfigManager()
    {
        if (Manager == null)
            throw new("Manager must be initialized");
        if (Config == null)
            throw new("Config must be initialized");

        ConfigManager = new(ref _config, SiteInteraction, Manager.SetupTextPieces);
    }

    public async Task HandleFileUpload(IReadOnlyList<IBrowserFile> files)
    {
        await HandleNewText(await FileImporter.ExtractFromBrowserFiles(files));
    }

    public async Task HandleNewText()
    {
        await HandleNewText(ReaderState.GetNew(ReaderStateSource.Internal, "Manual creation"));
    }

    private async Task HandleNewText(ReaderState readerState)
    {
        skipNextStateUpdate = true;
        await SetState(readerState);
        await Manager.UpdateSavedState();

        Log.Information("ReaderContext: HandleNewText");
    }

    public async Task HandleStateUpdated(ReaderState newState)
    {
        // must be at the start to prevent blocking of this update by the changes below, disabling the input field
        await stateTitle.SetText(newState.Title);
        await stateText.SetText(newState.Text);

        if (Manager != null)
            Manager.StopReadingTask();
        State = newState;

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

    public async Task HandlePasteTitle()
    {
        State.Title = await SiteInteraction.JSRuntime.InvokeAsync<string>("getClipboardContent");

        // should work without this line
        await HandleStateUpdated(State);

        await stateTitle.SetText(State.Title);
        await SiteInteraction.HandleSiteStateChanged();
    }

    public async Task HandlePasteText()
    {
        State.Text  = await SiteInteraction.JSRuntime.InvokeAsync<string>("getClipboardContent");

        // should work without this line
        await HandleStateUpdated(State);

        await stateText.SetText(State.Text);
        await SiteInteraction.HandleSiteStateChanged();
    }

    public async Task HandleTextChanged(string Text)
    {
        State.Text = Text.Trim();
        if (State.Text == string.Empty)
        {
            State.Text = ProductConstants.DefaultNewText;

            // should work without this line
            await SetState(State);
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
        State.Title = title.Trim();

        if (State.Text == string.Empty)
        {
            State.Text = ProductConstants.DefaultNewTitle;

            // should work without this line
            await SetState(State);
        }


        if (skipNextStateUpdate)
        {
            skipNextStateUpdate = false;
        }
        else
        {
            await Manager.RenameSavedState(State.Title, title);
        }
    }
}
