
using Microsoft.Extensions.Configuration;
//using Microsoft.EntityFrameworkCore;
using nexus.web.auth.Model;

namespace nexus.web.auth.Data
{
    public class NexusDB : DbContext
    {
        public IConfiguration Configuration;

        public NexusDB()
        {
            var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
            Configuration = builder.Build();

        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
        }

        //public DbSet<Product> Products { get; set; }

        //public DbSet<Users> Users { get; set; }

    }
}
