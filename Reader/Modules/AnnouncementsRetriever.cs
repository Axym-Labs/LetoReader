namespace Reader.Modules;

using Reader.Data.Storage;
using Reader.Data.Misc;
using Newtonsoft.Json;
using Reader.Modules.Logging;
using Newtonsoft.Json.Linq;
using System.Globalization;

public static class AnnouncementsRetriever
{

    public static async Task<List<Announcement>> GetAnnouncements()
    {
        try
        {
            HttpClient client = CreateClient();
            string announcementJsonString = await client.GetStringAsync(Constants.CentralAnnouncementsEndpoint);
            return JsonConvert.DeserializeObject<List<Announcement>>(announcementJsonString)!;
        } catch (Exception ex)
        {
            await Log.Error("AnnouncementsRetriever: GetAnnouncements - Error retrieving announcements", ex.Message, ex.StackTrace ?? string.Empty);
            return new List<Announcement>();
        }
    }

    public static async Task<string?> GetSpecialAnnouncement()
    {
        try
        {
            HttpClient client = CreateClient();
            string announcementJsonString = await client.GetStringAsync(Constants.CentralSpecialAnnouncementEndpoint);
            var obj = JsonConvert.DeserializeObject<JObject>(announcementJsonString);
            var CurrentSpecialAnnouncement = obj!["Text"]!.Value<string>()!;
            var ShowSpecialAnnouncementUntil = DateTime.MaxValue;
            try
            {
                ShowSpecialAnnouncementUntil = DateTime.ParseExact(obj!["Until"]!.Value<string>()!, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            }
            catch
            {
            }
            return ShowSpecialAnnouncementUntil >= DateTime.Now ? CurrentSpecialAnnouncement : null;

        }
        catch (Exception ex)
        {
            await Log.Error("AnnouncementsRetriever: GetAnnouncements - Error retrieving special announcement", ex.Message, ex.StackTrace ?? string.Empty);
            return null;
        }
    }

    private static HttpClient CreateClient()
    {
        HttpClient client = new HttpClient();
        client.Timeout = TimeSpan.FromSeconds(4);
        return client;
    }
}
