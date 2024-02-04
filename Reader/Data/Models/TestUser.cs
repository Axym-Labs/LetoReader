using Azure.Core;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Reader.Data.Models;

public class TestUser
{
    [Key]
    public int Id { get; set; }

    [Required]
    [Column("name")]
    public string? Name { get; set; }

    [Column("job")]
    public string? Job { get; set; }
    [Required]
    [Column("age")]
    public short? Age { get; set; }

    public ICollection<UserNotification>? Notifications { get; set; }
}