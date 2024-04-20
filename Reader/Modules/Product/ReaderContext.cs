namespace Reader.Modules.Product;

using Microsoft.JSInterop;
using Mono.TextTemplating;
using Reader.Data.Product;
using Reader.Data.Storage;

public class ReaderContext
{
    public ReaderManager ReaderManager { get; private set; }
    public ReaderConfigManager ReaderConfigManager { get; private set; }
    public ReaderState ReaderState { get; private set; }

    private SiteInteraction SiteInteraction { get; set; }

    public ReaderContext(ReaderManager readerManager, ReaderConfigManager readerConfigManager, ReaderState readerState, SiteInteraction siteInteraction)
    {
        ReaderManager = readerManager;
        ReaderConfigManager = readerConfigManager;
        ReaderState = readerState;
        SiteInteraction = siteInteraction;
    }



    private async Task SetStateValue(ReaderState newState)
    {
        if (ReaderManager == null)
            ReaderManager = new(ref newState, ReaderConfigManager);

        await HandleStateUpdated(newState);

        await SiteInteraction.HandleStateChanged();
    }

    private async Task HandleNewText()
    {

        ReaderState newState = new(ProductStorage.DefaultNewTitle, ProductStorage.DefaultNewText);

        await HandleStateUpdated(newState);

        await ReaderManager.UpdateSavedState();
    }

    private async Task HandleStateUpdated(ReaderState newState)
    {
        ReaderManager.StopReadingTask();
        ReaderManager.State = newState;
        ReaderManager.ClampPosition();
        ReaderState = newState;

        await stateTitle.SetText(_state.Title);
        await stateText.SetText(_state.Text);
    }

}
