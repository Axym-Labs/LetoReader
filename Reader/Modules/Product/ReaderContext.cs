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

    public async Task SetState(ReaderState newState)
    {
        if (Manager == null)
            ReInitManager();

        await HandleStateUpdated(newState);

        await SiteInteraction.HandleStateChanged();
    }

    public void InitializeReader()
    {
        ReInitManager();
    }

    private void ReInitManager()
    {
        Manager = new(ref _state, ref _config, SiteInteraction);
    }

    public async Task TriggerAfterFirstRenderEvents()
    {
        Manager.jsInteropAllowed = true;
        await LoadConfig();
        // start reader
        Manager.HandleStartStop();
    }

    private async Task LoadConfig()
    {
        string? loadedConfigStr = await SiteInteraction.JSRuntime.InvokeAsync<string?>("loadConfigurationStrIfExists");
        if (!string.IsNullOrEmpty(loadedConfigStr))
        {
            _config = JsonConvert.DeserializeObject<ReaderConfig>(loadedConfigStr!)!;
        }
    }

    public async Task HandleNewText()
    {
        ReaderState newState = ReaderState.GetNew();

        await HandleStateUpdated(newState);
        await Manager.UpdateSavedState();
    }

    public async Task HandleStateUpdated(ReaderState newState)
    {
        Manager.StopReadingTask();
        State = newState;
        // should work without this line
        //Manager.State = newState;
        Manager.ClampPosition();

        await stateTitle.SetText(State.Title);
        await stateText.SetText(State.Text);
    }

    public async Task HandlePasteTitle()
    {
        State.Title = await SiteInteraction.JSRuntime.InvokeAsync<string>("getClipboardContent");
        // should work without this line
        // await stateTitle.SetText(title);
    }

    public async Task HandlePasteText()
    {
        State.Title = await SiteInteraction.JSRuntime.InvokeAsync<string>("getClipboardContent");
        // should work without this line
        //await stateText.SetText(text);
    }

    public async Task HandleFileUpload(IReadOnlyList<IBrowserFile> files)
    {
        string importedText = await FileHelper.ExtractFromBrowserFiles(files);

        await stateText.SetText(importedText);
    }

}
