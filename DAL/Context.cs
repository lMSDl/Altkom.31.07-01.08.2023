using Microsoft.EntityFrameworkCore;
using Models;

namespace DAL
{
    public class Context : DbContext
    {
        public Context(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);

            //modelBuilder.HasChangeTrackingStrategy(ChangeTrackingStrategy.ChangedNotifications);
        }

        public bool RandomFail { get; set; }

        public override int SaveChanges()
        {
            ChangeTracker.Entries<IModifiedDate>()
                .Where(x => x.State == EntityState.Modified || x.State == EntityState.Added)
                .Select(x => x.Entity)
                .ToList()
                .ForEach(x => x.ModifiedDate = DateTime.Now);

            if (RandomFail && new Random((int)DateTime.Now.Ticks).Next(1, 50) == 1)
                throw new Exception();


            return base.SaveChanges();
        }
    }
}