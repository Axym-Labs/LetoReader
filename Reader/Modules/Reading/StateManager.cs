using Blazored.LocalStorage;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Reader.Data.Reading;

namespace Reader.Modules.Reading;

public class StateManager
{
    public List<ReaderState> ReaderStates { get; private set; } = default!;
    private ILocalStorageService localStorage = default!;

    public ReaderState CurrentState { get; set; } = default!;
    public string CurrentText = "This is the default text and shouldn't show up";

    public StateManager(ILocalStorageService localStorage)
    {
        this.localStorage = localStorage;
    }

    public async Task SaveStates()
    {
        await localStorage.SetItemAsync<List<ReaderState>>("STATES", ReaderStates);
    }

    public async Task AddState(ReaderState state, string content)
    {
        ReaderStates.Add(state);
        ReaderStates = ReaderStates.OrderByDescending(x => x.LastRead).ToList();
        await SaveStates();
        await SaveTextContent(state, content);
    }

    public async Task LoadSavedStates()
    {
        ReaderStates = (await localStorage.GetItemAsync<List<ReaderState>>("STATES"))!;
        // If no states present, create one
        if (ReaderStates.Count <= 0)
        {
            ReaderStates.Add(ReaderState.GetNew(ReaderStateSource.Internal, "Manual creation").Item1);
        }
    }

    public async Task<string> LoadReaderText(ReaderState state)
    {
        return (await localStorage.GetItemAsync<string>($"TEXTCONTENT:{state.Title}"))!;
    }

    public async Task SaveTextContent(ReaderState state, string content)
    {
        await localStorage.SetItemAsStringAsync($"TEXTCONTENT:{state.Title}", content);
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
        
        state.Title = newName;

        await SaveTextContent(state, text);
        await SaveStates();
    }


}
