using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Reader.Data.Product;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace Reader.Modules;

public class ReaderConfigManager
{
    public ReaderConfig Config { get; set; } = new();
    private IJSRuntime JSRuntime;
    private Action SetupTextPieces;

    public ReaderConfigManager(IJSRuntime JSRuntime, Action setupTextPieces) {
        this.JSRuntime = JSRuntime;
        SetupTextPieces = setupTextPieces;
    }

    public int FrontReadingSpeed
    {
        get => Config.ReadingSpeed;
        set
        {
            Config.ReadingSpeed = value;
            SaveConfig();
        }
    }

    public int FrontPeripheralCharsCount
    {
        get => Config.PeripheralCharsCount;
        set
        {
            Config.PeripheralCharsCount = value;
            SaveConfig();
        }
    }

    public int FrontPeripheralWordsBrightness
    {
        get => Config.PeripheralWordsBrightness;
        set
        {
            Config.PeripheralWordsBrightness = value;
            SaveConfig();
        }
    }

    public int FrontWordNavCount
    {
        get => Config.WordNavCount;
        set
        {
            Config.WordNavCount = value;
            SaveConfig();
        }
    }

    public int FrontTextSize
    {
        get => Config.TextSize;
        set
        {
            Config.TextSize = value;
            SaveConfig();
        }
    }

    public string FrontFont
    {
        get => string.Join(" ", Regex.Split(Config.Font.ToString(), @"(?<!^)(?=[A-Z])"));
        set
        {
            Config.Font = (Font)System.Enum.Parse(typeof(Font), value.Replace(" ", ""));
            SaveConfig();
        }
    }

    public List<string> FontOptions = Enum
        .GetValues(typeof(Font))
        .Cast<Font>()
        .Select(x => string.Join(" ", Regex.Split(x.ToString(), @"(?<!^)(?=[A-Z])")))
        .ToList();

    public int FrontFontWeight
    {
        get => Config.FontWeight;
        set
        {
            Config.FontWeight = value;
            SaveConfig();
        }
    }

    public int FrontWordCharLimit
    {
        get => Config.WordCharLimit;
        set
        {
            Config.WordCharLimit = value;
            SaveConfig();
            SetupTextPieces();
        }
    }

    public string FrontMiddleCharHighlightingColor
    {
        get => Config.MiddleCharHighlightingColor;
        set
        {
            Config.MiddleCharHighlightingColor = value;
            SaveConfig();
        }
    }

    private async Task SaveConfig() {
        await JSRuntime.InvokeVoidAsync("saveConfiguration", JsonConvert.SerializeObject(Config));
    }

    
}
