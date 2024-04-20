using Microsoft.JSInterop;

namespace Reader.Data.Product;

public class SiteInteraction
{
    public Func<Action, Task> InvokeAsync { get; private set; }
    public Action StateHasChanged { get; private set; }
    public IJSRuntime JSRuntime { get; private set; }

    public SiteInteraction(Func<Action, Task> invokeAsync, Action stateHasChanged, IJSRuntime JSRuntime)
    {
        InvokeAsync = invokeAsync;
        StateHasChanged = stateHasChanged;
        this.JSRuntime = JSRuntime;
    }

    public async Task HandleStateChanged()
    {
        await InvokeAsync(() => { StateHasChanged(); });
    }
}
