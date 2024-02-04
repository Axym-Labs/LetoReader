namespace Reader.Modules;

using Newtonsoft.Json;
using Reader.Data.Storage;



public class FileContentStorage
{
    public readonly Dictionary<string, dynamic> files = new();

    public FileContentStorage()
    {
        ReadAllJsonFiles(Constants.ContentDir);
    }

    public void ReadAllJsonFiles(string directoryPath)
    {
        string[] jsonFiles = Directory.GetFiles(directoryPath, "*.json");

        foreach (string filePath in jsonFiles)
        {
            string fileName = Path.GetFileName(filePath);
            string json = File.ReadAllText(filePath);
            dynamic? content = JsonConvert.DeserializeObject<dynamic>(json); // Deserialize using Newtonsoft.Json
            if (content != null)
                files.Add(fileName.Replace(".json", ""), content);
        }
    }
}
