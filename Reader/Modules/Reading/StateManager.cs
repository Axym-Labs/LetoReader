using Blazored.LocalStorage;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
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

    public ReaderState CurrentState { get; set; } = new(ProductConstants.DemoTitle, ReaderStateSource.Program, "Automatic initialization of demo text", DateTime.Now);

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
        CurrentText = await LoadReaderText(state);
        await TextHasChanged();
        await siteInteraction.HandleSiteStateChanged();
    }

    public async Task AddState(ReaderState state, string content, bool setAsSelected = true)
    {
        Log.Information("ReaderContext: AddState");

        // TODO
        //state.Title = GetUniqueTitle(state.Title);

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

        if (ReaderStates.Count == 0)
        {
            await AddState(new ReaderState(ProductConstants.DemoTitle, ReaderStateSource.Program), ProductConstants.DemoText, false);
        }

        Console.WriteLine(ReaderStates.Count);
        Console.WriteLine(ReaderStates[0].Title);

        await SwitchToState(ReaderStates[0]);
    }

    public async Task<string> LoadReaderText(ReaderState state)
    {
        return (await localStorage.GetItemAsync<string>($"TEXTCONTENT:{state.Title}"))!;
    }

    public async Task SaveState(ReaderState state, string content)
    {
        await SaveStates();
        await localStorage.SetItemAsStringAsync($"TEXTCONTENT:{state.Title}", TextHelper.Sanitize(content));
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
        var text = await LoadReaderText(state);
        await localStorage.RemoveItemAsync($"TEXTCONTENT:{state.Title}");
        
        state.Title = newName.Trim();

        if (newName == string.Empty)
            state.Title = ProductConstants.DefaultNewTitle;

        await SaveState(state, text);
        await TextHasChanged();
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

}
