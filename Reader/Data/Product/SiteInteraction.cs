using Microsoft.JSInterop;
using MudBlazor;

namespace Reader.Data.Product;

public class SiteInteraction
{
    public Func<Action, Task> InvokeAsync { get; private set; }
    private Action StateHasChanged { get; set; }
    public IJSRuntime JSRuntime { get; private set; }
    public bool JsAllowed { get; private set; }
    public ISnackbar Snackbar { get; private set; }

    public SiteInteraction(Func<Action, Task> invokeAsync, Action stateHasChanged, IJSRuntime JSRuntime, ISnackbar snackbar)
    {
        InvokeAsync = invokeAsync;
        StateHasChanged = stateHasChanged;
        this.JSRuntime = JSRuntime;
        JsAllowed = false;
        Snackbar = snackbar;
    }

    public void TriggerAfterRenderEvents()
    {
        JsAllowed = true;
    }

    public async Task HandleSiteStateChanged()
    {
        await InvokeAsync(() => { StateHasChanged(); });
    }
}
