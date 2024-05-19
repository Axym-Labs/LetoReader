using Blazored.LocalStorage;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.Identity.Client;
using Reader.Data.Reading;
using Reader.Data.Storage;
using Reader.Modules.Logging;

namespace Reader.Modules.Reading;

public class StateManager
{
    public List<ReaderState> ReaderStates { get; private set; } = new List<ReaderState>();
    private ILocalStorageService localStorage = default!;

    public ReaderState CurrentState { get; set; } = new("Default State", ReaderStateSource.Program, "Nah", DateTime.Now);

    public string _currentText = "This is a default state and shouldn't show up";

    public string CurrentText { get => _currentText; set => _currentText = value.Trim(); }

    public StateManager(ILocalStorageService localStorage)
    {
        this.localStorage = localStorage;
    }

    public async Task SaveStates()
    {
        await localStorage.SetItemAsync<List<ReaderState>>("STATES", ReaderStates);
    }


    public async Task SwitchToState(ReaderState state)
    {
        Log.Information("ReaderContext: SwitchState");

        await SaveStates();
        await SaveState(CurrentState, CurrentText);

        CurrentState = state;
        CurrentText = await LoadReaderText(state);
    }

    public async Task AddState(ReaderState state, string content)
    {
        Log.Information("ReaderContext: Adding State");

        ReaderStates.Add(state);
        ReaderStates = ReaderStates.OrderByDescending(x => x.LastRead).ToList();
        await SaveStates();
        await SaveState(state, content);
    }
    public async Task AddState(Tuple<ReaderState, string> state)
    {
        await AddState(state.Item1, state.Item2);
    }

    public async Task LoadSavedStates()
    {
        var states = (await localStorage.GetItemAsync<List<ReaderState>>("STATES"))!;

        if (states != null)
            ReaderStates = states;
        // If no states present, create one
        if (ReaderStates.Count <= 0)
        {
            await AddState(ReaderState.GetNew(ReaderStateSource.Internal, "Manual creation"));
        }

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
        ReaderStates.Remove(state);
        await localStorage.RemoveItemAsync($"TEXTCONTENT:{state.Title}");
        await SaveStates();
    }

    public async Task RenameState(ReaderState state, string newName)
    {
        var text = await LoadReaderText(state);
        await localStorage.RemoveItemAsync($"TEXTCONTENT:{state.Title}");
        
        state.Title = newName.Trim();

        if (newName == string.Empty)
            state.Title = ProductConstants.DefaultNewTitle;

        await SaveState(state, text);
        await SaveStates();
    }

}
