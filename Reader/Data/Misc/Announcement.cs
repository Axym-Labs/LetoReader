namespace Reader.Data.Misc;

public class Announcement
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public int ShowForDays { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public bool Priority { get; set; }
    public AnnouncementAction Action { get; set; }

    public Announcement(int id, DateTime date, int showForDays, string title, string description, bool priority, AnnouncementAction action)
    {
        Id = id;
        Date = date;
        ShowForDays = showForDays;
        Title = title;
        Description = description;
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
