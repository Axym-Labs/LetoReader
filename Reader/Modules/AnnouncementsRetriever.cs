namespace Reader.Modules;

using Reader.Data.Storage;
using Reader.Data.Misc;
using Newtonsoft.Json;

public static class AnnouncementsRetriever
{

    public static async Task<List<Announcement>> GetAnnouncements()
    {
        HttpClient client = new HttpClient();
        string announcementJsonString = await client.GetStringAsync(Constants.CentralAnnouncementsEndpoint);
        return JsonConvert.DeserializeObject<List<Announcement>>(announcementJsonString)!;
    }
}
