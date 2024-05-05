using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SH.DAL.Entities;

namespace SH.DAL
{
    public interface ICustomerDbContext
    {
        //DbSet<Performer> Performers { get; set; }
        //DbSet<Song> Songs { get; set; }

        //public abstract EntityEntry<TEntity> Update<TEntity>(TEntity entity)
        //    where TEntity : class;

        public abstract Task<int> SaveChangesAsync();
    }

    public class CustomerDbContext : DbContext
    {
        //public string DbPath { get; set; }
        public DbSet<Customer> Customers { get; set; } = null!;

        public CustomerDbContext(DbContextOptions<CustomerDbContext> options)
            : base(options)
        {
        }

        //public ProductContext(IConfiguration configuration)
        //{
        //    var path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        //    DbPath = Path.Join(path, configuration.GetConnectionString("ProductDbFilename"));
        //}

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            ////var path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            ////DbPath = Path.Join(path, "CustomerDb");

            //var path = Path.Join(
            //    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), 
            //    "CustomerDb");

            //options.UseSqlite($"Data Source={path}");
        }


        //// DB Context
        //builder.Services.AddDbContext<JukeboxDbContext>(
        //    DbContextOptions => DbContextOptions.UseSqlite(
        //        builder.Configuration["ConnectionStrings:JukeboxDBConnectionString"],
        //        b => b.MigrationsAssembly("Jukebox.DAL")));


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public Task<int> SaveChangesAsync()
        {
            return base.SaveChangesAsync();
        }
    }
}