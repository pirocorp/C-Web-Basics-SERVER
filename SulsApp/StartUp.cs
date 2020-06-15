namespace SulsApp
{
    using System.Collections.Generic;
    using Controllers;
    using Microsoft.EntityFrameworkCore;
    using SIS.HTTP;
    using SIS.MvcFramework;

    public class StartUp : IMvcApplication
    {
        public void ConfigureServices()
        {
            var db = new ApplicationDbContext();
            db.Database.Migrate();
        }

        public void Configure(IList<Route> routeTable)
        {
        }
    }
}
