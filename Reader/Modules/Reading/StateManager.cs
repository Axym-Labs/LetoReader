using Blazored.LocalStorage;
using Reader.Data.Reading;

namespace Reader.Modules.Reading;

public class StateManager
{
    public List<ReaderState> ReaderStates { get; private set; } = new();
    private ILocalStorageService localStorage = default!;
    public StateManager(ILocalStorageService localStorage)
    {
        this.localStorage = localStorage;
    }

    public async Task SaveStates()
    {
        await localStorage.SetItemAsync<List<ReaderState>>("STATES", ReaderStates);
    }

    public async Task LoadSavedStates()
    {
        ReaderStates = (await localStorage.GetItemAsync<List<ReaderState>>("STATES"))!;
    }

    public async Task<ReaderText> LoadReaderText(ReaderState state)
    {
        return new ReaderText() { Content = (await localStorage.GetItemAsync<string>($"TEXTCONTENT:{state.Title}"))! };
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
}
