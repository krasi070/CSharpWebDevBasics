namespace _03.ManyToManyRelation
{
    public class ManyToManyRelation
    {
        static void Main()
        {
            StudentDbContext db = new StudentDbContext();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
        }
    }
}