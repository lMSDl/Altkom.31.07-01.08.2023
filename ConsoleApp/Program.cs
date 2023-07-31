﻿
using DAL;
using Microsoft.EntityFrameworkCore;
using Models;
using System.Collections.ObjectModel;
using System.Security.Cryptography.X509Certificates;

var contextOptions = new DbContextOptionsBuilder<Context>()
    .UseSqlServer(@"Server=(local)\SQLEXPRESS;Database=EFCore;Integrated security=true")
    //Włączenie śledzenia zmian na podstawie proxy - wymaga specjalnego tworzenia obiektów (context.CreateProxy) i virtualizacji właściwości encji
    //.UseChangeTrackingProxies()
    //.LogTo(Console.WriteLine)
    .Options;

var context = new Context(contextOptions);
context.Database.EnsureDeleted();
context.Database.EnsureCreated();


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
    order.Products = new ObservableCollection<Product>( Enumerable.Range(1, new Random(i).Next(2, 10))
        .Select(x => new Product { Name = x.ToString(), Price = x * 0.32f })
        .ToList() );

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