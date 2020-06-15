namespace SulsApp.Services
{
    using System;
    using Models;

    public class ProblemsService : IProblemsService
    {
        private readonly ApplicationDbContext _db;

        public ProblemsService(ApplicationDbContext db)
        {
            this._db = db;
        }

        public void Create(string name, int points)
        {
            var problem = new Problem()
            {
                Name = name,
                Points = points,
            };

            this._db.Problems.Add(problem);
            this._db.SaveChanges();
        }
    }
}
