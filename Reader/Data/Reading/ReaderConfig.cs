using Blazored.LocalStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Reader.Data.Reading;

public class ReaderConfig
{
    public int ReadingSpeed = 300;
    public Font Font = Font.Poppins;
    public int FontWeight = 500;
    public int TextSize = 24;
    public int PeripheralCharsCount = 12;
    public int PeripheralWordsBrightness = 500;
    public int WordNavCount = 10;
    public int WordCharLimit = 30;
    public string MiddleCharHighlightingColor = "#FFFFFF";

    public static ReaderConfig GetDefault()
    {
        return new ReaderConfig();
    }

    public static async Task<ReaderConfig> LoadConfig(ILocalStorageService localStorage)
    {
        return (await localStorage.GetItemAsync<ReaderConfig>("readerConfig"))!;
    }
}

public enum Font
{
    Inter,
    InterTight,
    Merriweather,
    Poppins,
    OpenSans,
    RobotoCondensed,
}