using Blazored.LocalStorage;
using Blazored.LocalStorage.Exceptions;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Reader.Data.Product;
using Reader.Data.Reading;
using Reader.Data.Storage;
using Reader.Modules.Logging;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Reader.Modules.Reading;

public class StateManager
{
    public List<ReaderState> ReaderStates { get; private set; } = new List<ReaderState>();
    private ILocalStorageService localStorage = default!;

    public ReaderState CurrentState { get; set; } = new(ProductConstants.DemoTitle, ProductConstants.DemoText, ReaderStateSource.Program, "Automatic initialization of demo text", DateTime.Now);

    public string _currentText = ProductConstants.DemoText;

    public string CurrentText { get => _currentText; set => _currentText = value.Trim(); }
    private SiteInteraction siteInteraction;
    private Func<Task> TextHasChanged;
    public StateManager(ILocalStorageService localStorage, SiteInteraction site, Func<Task> updateFieldsFunc)
    {
        this.localStorage = localStorage;
        siteInteraction = site;
        TextHasChanged = updateFieldsFunc;
    }

    public async Task SaveStates()
    {
        await localStorage.SetItemAsStringAsync("STATES", JsonConvert.SerializeObject(ReaderStates));
    }

    public async Task SwitchToState(ReaderState state)
    {
        Log.Information("ReaderContext: SwitchState");

        await SaveStates();
        await SaveState(CurrentState, CurrentText);

        CurrentState = state;

        var text = await LoadReaderText(state);
        if (text == null)
        {
            await DeleteState(state);
            return;
        }

        CurrentText = text;

        await TextHasChanged();
        await siteInteraction.HandleSiteStateChanged();
    }

    public async Task AddState(ReaderState state, string content, bool setAsSelected = true)
    {
        Log.Information("ReaderContext: AddState");

        state.Title = GetUniqueTitle(state.Title);

        ReaderStates.Add(state);
        ReaderStates = ReaderStates.OrderByDescending(x => x.LastRead).ToList();

        await SaveState(state, content);
        if (setAsSelected)
        {
            await SwitchToState(state);
            await TextHasChanged();
        }

        await siteInteraction.HandleSiteStateChanged();
    }

    public async Task AddState(Tuple<ReaderState, string> state, bool setAsSelected = true)
    {
        await AddState(state.Item1, state.Item2, setAsSelected);
    }

    public async Task LoadSavedStates()
    {

        var stateStrings = (await localStorage.GetItemAsStringAsync("STATES"));
        List<ReaderState>? states = null;
        if (stateStrings != string.Empty && stateStrings != null)
            states = JsonConvert.DeserializeObject<List<ReaderState>>(stateStrings)!;

        if (states != null)
            ReaderStates = states;

        ReaderStates = ReaderStates.OrderByDescending(x => x.LastRead).ToList();

        if (ReaderStates.Count == 0)
        {
            await AddState(new ReaderState(ProductConstants.DemoTitle, ProductConstants.DemoText, ReaderStateSource.Program), ProductConstants.DemoText, false);
        }

        await SwitchToState(ReaderStates[0]);

        await MigrateOldStates();
    }

    public async Task<string?> LoadReaderText(ReaderState state)
    {
        return await localStorage.GetItemAsync<string>($"TEXTCONTENT:{state.Title}");
    }

    public async Task SaveState(ReaderState state, string content)
    {
        try
        {
        await SaveStates();
        await localStorage.SetItemAsStringAsync($"TEXTCONTENT:{state.Title}", TextHelper.Sanitize(content));
        } catch (BrowserStorageDisabledException e)
        {
            Log.Error("ReaderContext: SaveState: Browser storage disabled", e.Message, e.StackTrace ?? string.Empty);
            siteInteraction.Snackbar.Add("The browser storage is disabled. Please enable it to use this feature.", MudBlazor.Severity.Error);
        } catch (Exception e)
        {
            Log.Error("ReaderContext: SaveState - Could not save state.", e.Message, e.StackTrace ?? string.Empty);
            siteInteraction.Snackbar.Add("An error occurred while saving the reading state. Your browser storage may be full. Delete an old text and try again.", MudBlazor.Severity.Error);
        }
    }

    public async Task DeleteState(ReaderState state)
    {
        Log.Information("ReaderContext: DeleteState");

        ReaderStates.Remove(state);
        await localStorage.RemoveItemAsync($"TEXTCONTENT:{state.Title}");
        await SaveStates();
        await siteInteraction.HandleSiteStateChanged();
    }

    public async Task RenameState(ReaderState state, string newName)
    {
        string? text = await LoadReaderText(state)!;

        await localStorage.RemoveItemAsync($"TEXTCONTENT:{state.Title}");
        
        state.Title = newName.Trim();

        if (newName == string.Empty)
            state.Title = ProductConstants.DefaultNewTitle;
        if (text != null)
            await SaveState(state, text);
        await siteInteraction.HandleSiteStateChanged();
    }

    private string GetUniqueTitle(string title)
    {
        var newTitle = title;
        var i = 1;
        while (ReaderStates.Any(x => x.Title == newTitle))
        {
            newTitle = $"{title} ({i})";
            i++;
        }

        return newTitle;
    }

    private async Task MigrateOldStates()
    {
        var keys = await localStorage.KeysAsync();
        
        foreach (var key in keys)
        {
            if (key.StartsWith("TEXTSTATE:"))
            {
                var obj = JsonConvert.DeserializeObject<JObject>((await localStorage.GetItemAsStringAsync(key))!);

                try
                {
                    await AddState(ReaderState.ImportFromJson(obj!), false);
                } catch (Exception e)
                {
                    siteInteraction.Snackbar.Add("The data of an old reading state is corrupted. It will be ignored, but is recoverable by accessing your browser storage.", MudBlazor.Severity.Error);
                    Log.Error("ReaderContext: MigrateOldStates: Could not import state", e.Message, e.StackTrace ?? string.Empty);

                    await localStorage.SetItemAsStringAsync($"CORRUPTED-{key}", (await localStorage.GetItemAsStringAsync(key))!);
                }
                await localStorage.RemoveItemAsync(key);


            }
        }
    }
}
