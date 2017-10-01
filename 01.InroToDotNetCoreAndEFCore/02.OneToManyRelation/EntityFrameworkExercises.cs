namespace _02.OneToManyRelation
{
    using System;

    public class EntityFrameworkExercises
    {
        // 2. One-to-Many Relation
        // 3. Self-Referenced Table
        public static void Main()
        {
            EmployeeDbContext db = new EmployeeDbContext();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
        }
    }
}