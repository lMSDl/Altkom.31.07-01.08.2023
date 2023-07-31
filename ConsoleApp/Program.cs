
using DAL;
using Microsoft.EntityFrameworkCore;
using Models;

var contextOptions = new DbContextOptionsBuilder<Context>()
    .UseSqlServer(@"Server=(local)\SQLEXPRESS;Database=EFCore;Integrated security=true")
    //.LogTo(Console.WriteLine)
    .Options;

var context = new Context(contextOptions);
context.Database.EnsureDeleted();
context.Database.EnsureCreated();


var order = new Order();
var product = new Product() { Name = "Sałata", Price = 15 };

order.Products.Add(product);


Console.WriteLine("Order przed dodaniem do kontekstu: " + context.Entry(order).State);
Console.WriteLine("Product przed dodaniem do kontekstu: " + context.Entry(product).State);

context.Add(order);

Console.WriteLine("Order po dodaniu do kontekstu: " + context.Entry(order).State);
Console.WriteLine("Product po dodaniu do kontekstu: " + context.Entry(product).State);

context.SaveChanges();

Console.WriteLine("Order po zapisie: " + context.Entry(order).State);
Console.WriteLine("Product po zapisie: " + context.Entry(product).State);

order.DateTime = DateTime.Now;

Console.WriteLine("Order po zmianie daty: " + context.Entry(order).State);
Console.WriteLine("Order DateTime zmodywikowany? " + context.Entry(order).Property(x => x.DateTime).IsModified);
Console.WriteLine("Order Products zmodywikowany? " + context.Entry(order).Collection(x => x.Products).IsModified);

Console.WriteLine("Product po zmianie daty: " + context.Entry(product).State);

context.Remove(product);

Console.WriteLine("Order po usunięciu produktu: " + context.Entry(order).State);
Console.WriteLine("Product po usunięciu produktu: " + context.Entry(product).State);

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
    order.Products = Enumerable.Range(1, new Random(i).Next(2, 10))
        .Select(x => new Product { Name = x.ToString(), Price = x * 0.32f })
        .ToList();

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