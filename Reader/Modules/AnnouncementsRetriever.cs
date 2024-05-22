namespace Reader.Modules;

using Reader.Data.Storage;
using Reader.Data.Misc;
using Newtonsoft.Json;
using Reader.Modules.Logging;

public static class AnnouncementsRetriever
{

    public static async Task<List<Announcement>> GetAnnouncements()
    {
        try
        {
            HttpClient client = new HttpClient();
            string announcementJsonString = await client.GetStringAsync(Constants.CentralAnnouncementsEndpoint);
            return JsonConvert.DeserializeObject<List<Announcement>>(announcementJsonString)!;
        } catch (Exception ex)
        {
            Log.Error("AnnouncementsRetriever: GetAnnouncements - Error retrieving announcements", ex.Message, ex.StackTrace ?? string.Empty);
            return new List<Announcement>();
        }
    }
}
