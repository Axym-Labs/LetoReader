namespace Reader.Modules.Db;

using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using Reader.Data.Models;
using Microsoft.EntityFrameworkCore;

public class TestDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public Microsoft.EntityFrameworkCore.DbSet<TestUser> TestUsers { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(GetConnectionString());
    }

    private static string GetConnectionString()
    {
#if DEBUG
        return Environment.GetEnvironmentVariable("Reader_Database_ConnectionStringDevelopment")!;
#else
        return Environment.GetEnvironmentVariable("Reader_Database_ConnectionStringProduction")!;
#endif
    }

    // protected override void OnModelCreating(DbModelBuilder modelBuilder)
    // {
    //     modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
    //     base.OnModelCreating(modelBuilder);

    //     modelBuilder.Entity<TestUser>().ToTable("TestUser");
    //     modelBuilder.Entity<UserNotification>().ToTable("UserNotifications");

    //     // seed content here
    //     new DbInitializer(modelBuilder).Seed();
    // }
}

