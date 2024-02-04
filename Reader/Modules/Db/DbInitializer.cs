
namespace Reader.Modules.Db;

using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;

public class DbInitializer
{
    private readonly DbModelBuilder modelBuilder;

    public DbInitializer(DbModelBuilder modelBuilder)
    {
        this.modelBuilder = modelBuilder;
    }

    public void Seed()
    {
        //modelBuilder.Entity<User>().HasData(
        //       new User() { Id = 1.... },
        //       new User() { Id = 2.... },
        //);
    }
}
