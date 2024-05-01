using MudBlazor;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Reader.Data.Misc;

public class Announcement
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public int ShowForDays { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Type { get; set; }
    public Color Color => Type switch
    {
        "Info" => Color.Info,
        "Success" => Color.Success,
        "Important" => Color.Warning,
        "Critical" => Color.Error,
        _ => Color.Primary
    };
    public bool Priority { get; set; }
    public AnnouncementAction Action { get; set; }

    public Announcement(int id, DateTime date, int showForDays, string title, string description, string type, bool priority, AnnouncementAction action)
    {
        Id = id;
        Date = date;
        ShowForDays = showForDays;
        Title = title;
        Description = description;
        Type = type;
        Priority = priority;
        Action = action;
    }

    public bool IsExpired()
    {
        return Date.AddDays(ShowForDays) < DateTime.Now;
    }
}

public class AnnouncementAction
{
    public string Url { get; set; }
    public string Title { get; set; }

    public AnnouncementAction(string url, string title)
    {
        Url = url;
        Title = title;
    }
}
