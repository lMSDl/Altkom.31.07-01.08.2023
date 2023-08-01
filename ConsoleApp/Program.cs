
using DAL;
using Microsoft.EntityFrameworkCore;
using Models;
using NetTopologySuite.Geometries;
using System.Collections.ObjectModel;
using System.Security.Cryptography.X509Certificates;

var contextOptions = new DbContextOptionsBuilder<Context>()
    .UseSqlServer(@"Server=(local)\SQLEXPRESS;Database=EFCore;Integrated security=true;Encrypt=False", x => x.UseNetTopologySuite())
    //Włączenie śledzenia zmian na podstawie proxy - wymaga specjalnego tworzenia obiektów (context.CreateProxy) i virtualizacji właściwości encji
    //.UseChangeTrackingProxies()

    //Włączenie opóźnionego ładowania - wymaga wirtualizacji właściwości referencji
    //.UseLazyLoadingProxies()
    .LogTo(Console.WriteLine)
    .Options;

using (var context = new Context(contextOptions))
{
    context.Database.EnsureDeleted();
    context.Database.Migrate();
}



for (int i = 0; i < 20; i++)
{

    using (var context = new Context(contextOptions))
    {

        var order = new Order() { OrderType = (OrderType)(i % 3), Role = (Roles)(i % 4) };
        order.DeliveryPoint = new NetTopologySuite.Geometries.Point(52 - i/10f, 21 - i/10f) { SRID = 4326 };


        var product = new Product() { Name = "Kapusta " + i };
        product.Details = new ProductDetails { Weight = i, Height = i * i, Width = i * i * i };
        order.Products.Add(product);
        context.Add(order);
        context.SaveChanges();
    }
}



using (var context = new Context(contextOptions))
{
    var order = new Order { Id = 1 };
    context.Attach(order);

    var product1 = new Product() { Name = "Kapusta ", Order = order };
    var product2 = new Product() { Name = "Kapusta ", Order = order };
    var product3 = new Product() { Name = "Kapusta ", Order = order };


    context.AddRange(new[] { product3, product1, product2 });

    context.SaveChanges();
}

    using (var context = new Context(contextOptions))
{

    var order = context.Set<Order>().Where(x => x.Role.HasFlag(Roles.Read)).ToList();

}

using (var context = new Context(contextOptions))
{

    var product = context.Set<Product>().First();
    var products = context.Set<Product>().Include(x => x.Details).Where(x => EF.Property<int>(x, "OrderId") == 1).ToList();

    context.Entry(product).Property<bool>("IsDeleted").CurrentValue = true;
    context.SaveChanges();

}

using (var context = new Context(contextOptions))
{
    var multiplier = "-1.1";
    //var multiplier = "-1.1; DROP TABLE Products";
    //context.Database.ExecuteSqlRaw("EXEC ChangePrice " + multiplier);

    //context.Database.ExecuteSqlRaw("EXEC ChangePrice @p0", multiplier);
    context.Database.ExecuteSqlInterpolated($"EXEC ChangePrice {multiplier}");

    var result = context.Set<OrderSummary>().FromSqlRaw("EXEC OrderSummary @p0", 3).ToList();

    result = context.Set<OrderSummary>().ToList();
}






using (var context = new Context(contextOptions))
{
    var order = context.Set<Order>().OrderBy(x => x.Id).First();

    var point = new Point(52, 21) { SRID = 4326 };

    var distance = point.Distance(order.DeliveryPoint);

    var intersect = point.Intersects(order.DeliveryPoint);

    var polygon = new Polygon(new LinearRing(new Coordinate[] { new Coordinate(52, 21),
                                                                new Coordinate(51, 20),
                                                                new Coordinate(52, 19),
                                                                new Coordinate(53, 20),
                                                                new Coordinate(52, 21)}))
    { SRID = 4326 };

    intersect = polygon.Intersects(order.DeliveryPoint);


    var orders = context.Set<Order>().Where(x => x.DeliveryPoint.Intersects(polygon)).ToList();
    orders = context.Set<Order>().OrderBy(x => x.DeliveryPoint.Distance(point)).ToList();
    orders = context.Set<Order>().Where(x => point.IsWithinDistance(x.DeliveryPoint, 0.5)).ToList();
}














    static void ChangeTracker(Context context)
{
    //context.ChangeTracker.AutoDetectChangesEnabled = false;

    var order = new Order();
    var product = new Product() { Name = "Sałata", Price = 15 };

    /*var order = context.CreateProxy<Order>();
    var product = context.CreateProxy<Product> (x =>  { x.Name = "Sałata"; x.Price = 15; });*/


    order.Products.Add(product);


    Console.WriteLine("Order przed dodaniem do kontekstu: " + context.Entry(order).State);
    Console.WriteLine("Product przed dodaniem do kontekstu: " + context.Entry(product).State);

    //context.Add(order);
    context.Attach(order);

    Console.WriteLine("Order po dodaniu do kontekstu: " + context.Entry(order).State);
    Console.WriteLine("Product po dodaniu do kontekstu: " + context.Entry(product).State);

    context.SaveChanges();

    Console.WriteLine("Order po zapisie: " + context.Entry(order).State);
    Console.WriteLine("Product po zapisie: " + context.Entry(product).State);

    context.Entry(order).State = EntityState.Detached;
    order.DateTime = DateTime.Now;
    context.Entry(order).Property(x => x.DateTime).IsModified = true;
    context.Attach(order);


    Console.WriteLine("Order po zmianie daty: " + context.Entry(order).State);
    Console.WriteLine("Order DateTime zmodywikowany? " + context.Entry(order).Property(x => x.DateTime).IsModified);
    Console.WriteLine("Order Products zmodywikowany? " + context.Entry(order).Collection(x => x.Products).IsModified);

    Console.WriteLine("Product po zmianie daty: " + context.Entry(product).State);

    context.Remove(product);

    Console.WriteLine("Order po usunięciu produktu: " + context.Entry(order).State);
    Console.WriteLine("Product po usunięciu produktu: " + context.Entry(product).State);

    context.Entry(product).State = EntityState.Unchanged;

    context.SaveChanges();

    Console.WriteLine("Order po zapisie: " + context.Entry(order).State);
    Console.WriteLine("Product po zapisie: " + context.Entry(product).State);

    //wyłączenie automatycznego wykrywania zmian
    //AutoDetectChanges działa w przypadku wywołania Entries, Local, SaveChanges
    context.ChangeTracker.AutoDetectChangesEnabled = false;

    order = new Order();
    product = new Product() { Name = "Kapusta", Price = 10 };
    order.Products.Add(product);
    product = new Product() { Name = "Masło", Price = 11 };
    order.Products.Add(product);


    Console.WriteLine("Przed dodaniem do kontekstu");
    Console.WriteLine(context.ChangeTracker.DebugView.ShortView);

    context.Add(order);

    Console.WriteLine("Po dodaniu do kontekstu");
    Console.WriteLine(context.ChangeTracker.DebugView.LongView);

    context.SaveChanges();

    Console.WriteLine("Po zapisie");
    Console.WriteLine(context.ChangeTracker.DebugView.ShortView);

    order.DateTime = DateTime.Now;

    Console.WriteLine("Po modyfikacji daty");

    //_  = context.Set<Order>().Local;
    //context.Entry(order);
    //context.ChangeTracker.DetectChanges();
    context.Entry(order).Property(x => x.DateTime).IsModified = true;
    Console.WriteLine(context.ChangeTracker.DebugView.LongView);

    context.Remove(product);

    Console.WriteLine("Po usunięciu produktu");
    Console.WriteLine(context.ChangeTracker.DebugView.ShortView);

    context.SaveChanges();

    Console.WriteLine("Po zapisie");
    Console.WriteLine(context.ChangeTracker.DebugView.ShortView);


    context.ChangeTracker.AutoDetectChangesEnabled = true;

    for (int i = 0; i < 3; i++)
    {
        order = new Order() { DateTime = DateTime.Now.AddMinutes(-i * 64) };
        order.Products = new ObservableCollection<Product>(Enumerable.Range(1, new Random(i).Next(2, 10))
            .Select(x => new Product { Name = x.ToString(), Price = x * 0.32f })
            .ToList());

        context.Add(order);
    }

    context.ChangeTracker.DetectChanges();
    Console.WriteLine(context.ChangeTracker.DebugView.ShortView);
    Console.WriteLine("-----");
    Console.WriteLine(context.ChangeTracker.DebugView.LongView);
    context.SaveChanges();
    Console.WriteLine(context.ChangeTracker.DebugView.ShortView);
    Console.WriteLine("-----");
    Console.WriteLine(context.ChangeTracker.DebugView.LongView);



    context.ChangeTracker.Clear();


    var products = new List<Product>()
{
    new Product() { Name = "P1", Order = new Order { Id = 60, DateTime = DateTime.Now } },
    new Product() { Name = "P2", Order = new Order { } }
};

    //context.AttachRange(products);

    foreach (var p in products)
    {
        context.ChangeTracker.TrackGraph(p, entityEntry =>
        {
            if (entityEntry.Entry.IsKeySet)
            {
                entityEntry.Entry.State = EntityState.Modified;
            }
            else
                entityEntry.Entry.State = EntityState.Added;

            entityEntry.Entry.Properties.Where(x => x.Metadata.ClrType == typeof(DateTime)).ToList().ForEach(x => x.IsModified = false);
        });
    }

    Console.WriteLine(context.ChangeTracker.DebugView.LongView);

    Console.ReadLine();
}

static void ConcurrencyToken(Context context)
{
    ChangeTracker(context);

    context.ChangeTracker.Clear();

    var order = context.Set<Order>().First();

    order.DateTime = DateTime.Now;
    context.SaveChanges();


    var product = context.Set<Product>().First();
    product.Price = 10;

    var saved = false;
    do
    {
        try
        {
            context.SaveChanges();
            saved = true;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            foreach (var entry in ex.Entries)
            {
                //wartości jakie chcemy wprowadzić do bazy
                var currentValues = entry.CurrentValues;
                //wartości jakie pobraliśmy z bazy (historyczne)
                var originalValues = entry.OriginalValues;
                //wartości jakie aktualnie są w bazie danych
                var databseValues = entry.GetDatabaseValues();

                switch (entry.Entity)
                {
                    case Product:
                        var property = currentValues.Properties.Single(x => x.Name == nameof(Product.Price));
                        var currentPrice = (float)currentValues[property]!;
                        var originalPrice = (float)originalValues[property]!;
                        var databasePrice = (float)databseValues![property]!;

                        currentValues[property] = databasePrice + (currentPrice - originalPrice);

                        break;
                }

                entry.OriginalValues.SetValues(databseValues);
            }
        }
    } while (!saved);
}

static void Transactions(DbContextOptions<Context> contextOptions, bool randomFail)
{
    var products = Enumerable.Range(100, 50).Select(x => new Product { Name = $"Product {x}", Price = 1.23f * x }).ToList();
    var orders = Enumerable.Range(0, 5).Select(x => new Order { DateTime = DateTime.Now.AddMinutes(-1.23f * x) }).ToList();




    using (var context = new Context(contextOptions))
    {
        context.RandomFail = randomFail;

        using var transaction = context.Database.BeginTransaction();

        //using (var context2 = new Context(contextOptions))
        //    context2.Database.UseTransaction(transaction.GetDbTransaction());

        for (int i = 0; i < orders.Count; i++)
        {
            string savepointName = i.ToString();
            transaction.CreateSavepoint(savepointName);

            try
            {
                var order = orders[i];
                context.Add(order);
                context.SaveChanges();

                var subProducts = products.Skip(i * 10).Take(10).ToList();

                foreach (var product in subProducts)
                {
                    product.Order = order;

                    context.Add(product);
                    context.SaveChanges();
                }
            }
            catch
            {
                transaction.RollbackToSavepoint(savepointName);
            }

            context.ChangeTracker.Clear();
        }

        transaction.Commit();
    }
}

static void DataLoading(DbContextOptions<Context> contextOptions)
{
    Transactions(contextOptions, false);

    Product product;

    using (var context = new Context(contextOptions))
    {
        //Eager Loading
        product = context.Set<Product>().Include(x => x.Order).ThenInclude(x => x.Products).First();
    }

    using (var context = new Context(contextOptions))
    {
        product = context.Set<Product>().First();
        //Explicit Loading
        context.Entry(product).Reference(x => x.Order).Load();
        context.Entry(product.Order).Collection(x => x.Products).Load();

        //context.Set<Order>().Load();
        //context.Set<Product>().Load();
    }

    using (var context = new Context(contextOptions))
    {
        //Lazy Loading
        product = context.Set<Product>().First();


    }
    Console.ReadLine();
}

static void QueryFilters(DbContextOptions<Context> contextOptions)
{
    using (var context = new Context(contextOptions))
    {
        var product = context.Set<Product>().First();

        //product.IsDeleted = true;
        context.Entry(product).Property<bool>("IsDeleted").CurrentValue = true;

        context.SaveChanges();
    }

    using (var context = new Context(contextOptions))
    {
        var product1 = context.Set<Product>()/*.IgnoreQueryFilters()*/.First();
        //var product2 = context.Set<Product>().Where(x => !x.IsDeleted).First();

        context.Entry(product1.Order).Collection(x => x.Products).Load();
    }
}

static void Temporal(DbContextOptions<Context> contextOptions)
{
    Transactions(contextOptions, false);


    using (var context = new Context(contextOptions))
    {
        //var product = context.Set<Product>().OrderBy(x => x.Id).First();
        var product = Context.GetFirstProduct(context);

        product.Name = "samochodzik";
        context.SaveChanges();
    }

    Thread.Sleep(5000);

    using (var context = new Context(contextOptions))
    {
        //var product = context.Set<Product>().OrderBy(x => x.Id).First();
        var product = Context.GetFirstProduct(context);

        product.Name = "samolocik";
        context.SaveChanges();
    }

    Thread.Sleep(5000);

    using (var context = new Context(contextOptions))
    {
        //var product = context.Set<Product>().OrderBy(x => x.Id).First();
        var product = Context.GetFirstProduct(context);

        product.Name = "";
        context.SaveChanges();
    }

    Thread.Sleep(5000);

    using (var context = new Context(contextOptions))
    {
        //var product = context.Set<Product>().OrderBy(x => x.Id).First();
        var product = Context.GetFirstProduct(context);
        context.Remove(product);
        context.SaveChanges();
    }

    using (var context = new Context(contextOptions))
    {
        var products = context.Set<Product>().TemporalAll().ToList();

        var product1 = context.Set<Product>().TemporalAsOf(DateTime.UtcNow.AddSeconds(-11)).OrderBy(x => x.Id).First();
        var product2 = context.Set<Product>().TemporalAsOf(DateTime.UtcNow.AddSeconds(-6)).OrderBy(x => x.Id).First();

        var data = context.Set<Product>().TemporalAll().Select(x => new { x, FROM = EF.Property<DateTime>(x, "From"), To = EF.Property<DateTime>(x, "To") }).ToList();
    }


    using (var context = new Context(contextOptions))
    {
        var orders = Context.GetOrdersByDateRange(context, DateTime.Now.AddDays(-1), DateTime.Now).ToList();
    }
}