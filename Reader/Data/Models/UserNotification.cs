namespace Reader.Data.Models;

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

public class UserNotification
{
    [Key]
    public int NotificationId { get; set; }
    [Required]
    [ForeignKey("TestUser")]
    public int Id { get; set; }
    [Required]
    public string Message { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public NotificationStatus Status { get; set; } = NotificationStatus.Unread;
    public NotificationPriority Priority { get; set; } = NotificationPriority.Low;
    public string? Source { get; set; }
    public string? ActionLink { get; set; }
    public string? Metadata { get; set; }
    public DateTime? ExpirationDate { get; set; }

    public TestUser TestUser { get; set; } = default!;

    public UserNotification(int userId, string message, NotificationPriority priority, string? source=null, string? actionLink=null, string? metadata=null, DateTime? expirationDate=null)
    {
        Id = userId;
        Message = message;
        Timestamp = DateTime.Now;
        Priority = priority;
        Source = source;
        ActionLink = actionLink;
        Metadata = metadata;
        ExpirationDate = expirationDate;
    }

    public UserNotification() { }
}

public enum NotificationStatus
{
    Unread,
    Read,
    Deleted
}

public enum NotificationPriority
{
    Low,
    Medium,
    High
}
