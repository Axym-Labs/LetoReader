namespace Reader.Data;

using Newtonsoft.Json;


public class AppConfig
{
    public bool TestSetting { get; set; } = default!;
    //public bool DatabaseSupported { get; set; } = default!;
    //public AppDbConfig Database { get; set; } = default!;

    public static AppConfig GetFromJsonFile(string configFileName)
    {
        var fileContents = File.ReadAllText(configFileName);
        return JsonConvert.DeserializeObject<AppConfig>(fileContents)!;
    }
}

//public class AppDbConfig
//{
//    public string ConnectionString { get; set; } = default!;
//    public AppDbUserConfig AppDbUserConfig { get; set; } = default!;
//}

//public class AppDbUserConfig
//{
//    public float MaxUploadTotalMb { get; set; } = default!;
//    public float MaxUploadFileMb { get; set; } = default!;
//}