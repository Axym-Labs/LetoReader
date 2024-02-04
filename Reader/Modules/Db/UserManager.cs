using Reader.Data.Models;

namespace Reader.Modules.Db;

public class UserManager
{

    private TestDbContext db = default!;

    public UserManager(TestDbContext db)
    {
        this.db = db;
    }

    public bool InsertOne(string Name, string Job, short Age)
    {
        try
        {
            TestUser newPerson = new TestUser { Name = Name, Job = Job, Age = Age };
            db.TestUsers.Add(newPerson);
            db.SaveChanges();
            return true;
        }
        catch { return false; }
    }

    public List<TestUser> ReadAll()
    {
        return db.TestUsers.ToList();
    }
}