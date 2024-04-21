namespace Reader.Modules.Product;

using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using MudBlazor;
using Newtonsoft.Json;
using Reader.Data.Product;
using Reader.Data.Storage;

public class ReaderContext
{
    public ReaderManager Manager { get; private set; }
    public ReaderConfigManager ConfigManager { get; private set; }

    private ReaderState _state = default!;
    public ReaderState State { get => _state; set => _state = value; }

    private ReaderConfig _config = default!;
    public ReaderConfig Config { get => _config; set => _config = value; }

    public MudTextField<string> stateTitle { get; set; } = new();
    public MudTextField<string> stateText { get; set; } = new();

    private SiteInteraction SiteInteraction { get; set; }

    public ReaderContext(SiteInteraction siteInteraction)
    {
        SiteInteraction = siteInteraction;
    }

    public async Task TriggerOnInitializedEvents()
    {
        // set default state and config for initialization of reader
        await SetState(ReaderState.GetDemo());
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
        if (State.Title == ReaderState.GetDemo().Title)
            Manager.StartReadingTask();
    }

    private async Task LoadConfig()
    {
        string? loadedConfigStr = await SiteInteraction.JSRuntime.InvokeAsync<string?>("loadConfigurationStrIfExists");
        if (!string.IsNullOrEmpty(loadedConfigStr))
        {
            _config = JsonConvert.DeserializeObject<ReaderConfig>(loadedConfigStr!)!;
            InitializeConfigManager();
        }
    }

    public async Task SetState(ReaderState newState)
    {
        await HandleStateUpdated(newState);
        await SiteInteraction.HandleSiteStateChanged();
    }

    public async Task SetConfig(ReaderConfig newConfig)
    {
        Config = newConfig;
        await SiteInteraction.HandleSiteStateChanged();
    }

    private void InitializeReader()
    {
        if (State == null)
            throw new("State must be initialized");
        if (Config == null)
            throw new("Config must be initialized");

        Manager = new(ref _state, ref _config, SiteInteraction);
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
        ReaderState newState = ReaderState.GetNew();

        await HandleStateUpdated(newState);
        await Manager.UpdateSavedState();
    }

    public async Task HandleStateUpdated(ReaderState newState)
    {
        if (Manager != null)
            Manager.StopReadingTask();
        State = newState;
        await stateTitle.SetText(State.Title);
        await stateText.SetText(State.Text);
        // should work without this line
        //Manager.State = newState;
        if (Manager != null)
            Manager.ClampPosition();

        // should work without this line
        //await stateTitle.SetText(State.Title);
        //await stateText.SetText(State.Text);
    }

    public async Task HandlePasteTitle()
    {
        State.Title = await SiteInteraction.JSRuntime.InvokeAsync<string>("getClipboardContent");
        await stateTitle.SetText(State.Title);
        await SiteInteraction.HandleSiteStateChanged();
    }

    public async Task HandlePasteText()
    {
        State.Text  = await SiteInteraction.JSRuntime.InvokeAsync<string>("getClipboardContent");
        await stateText.SetText(State.Text);
        await SiteInteraction.HandleSiteStateChanged();
    }

    public async Task HandleFileUpload(IReadOnlyList<IBrowserFile> files)
    {
        string importedText = await FileHelper.ExtractFromBrowserFiles(files);

        await stateText.SetText(importedText);
    }

}
