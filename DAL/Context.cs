using DAL.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Models;
using Pluralize.NET.Core;

namespace DAL
{
    public class Context : DbContext
    {
        public Context() { }

        public Context(DbContextOptions options) : base(options)
        {
        }


        public static Func<Context, Product> GetFirstProduct { get; } =
            EF.CompileQuery((Context context) => context.Set<Product>().OrderBy(x => x.Id).First());

        public static Func<Context, DateTime, DateTime, IEnumerable<Order>> GetOrdersByDateRange { get; } =
            EF.CompileQuery((Context context, DateTime from, DateTime to) =>
                context.Set<Order>()
            .AsNoTracking()
            .Include(x => x.Products)
            .Where(x => x.DateTime >= from)
            .Where(x => x.DateTime <= to));


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if(!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer();
            }

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);

            //modelBuilder.HasChangeTrackingStrategy(ChangeTrackingStrategy.ChangedNotifications);

            modelBuilder.Model.GetEntityTypes()
                .SelectMany(x => x.GetProperties())
                .Where(x => x.Name == "Klucz")
                .ToList()
                .ForEach(x =>
                {
                    x.IsNullable = false;
                    x.DeclaringEntityType.SetPrimaryKey(x);
                });

            modelBuilder.Model.GetEntityTypes()
                .ToList()
                .ForEach(x =>
                {
                    x.SetTableName(new Pluralizer().Pluralize(x.GetDefaultTableName()));
                });

            modelBuilder.Model.GetEntityTypes()
                .SelectMany(x => x.GetProperties())
                .Where(x => x.PropertyInfo?.PropertyType == typeof(string))
                .ToList()
                .ForEach(x =>
                {
                    x.IsNullable = true;
                    x.SetColumnName("s_" + x.GetDefaultColumnName());
                    if (x.PropertyInfo.CanWrite)
                        x.SetValueConverter(new StringConverter());
                });

            modelBuilder.HasSequence<int>("OrderNumber")
                .StartsAt(100)
                .HasMax(999)
                .HasMin(0)
                .IsCyclic()
                .IncrementsBy(333);

            modelBuilder.UsePropertyAccessMode(PropertyAccessMode.PreferField);
        }


        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            base.ConfigureConventions(configurationBuilder);


            configurationBuilder.Properties<DateTime>().HavePrecision(5);

            //configurationBuilder.Conventions.Remove(typeof(KeyDiscoveryConvention));
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