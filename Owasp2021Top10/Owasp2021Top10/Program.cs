using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;

using System.Security.Cryptography;
using System.Text;
using Owasp2021Top10.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
//builder.Services.AddDbContext<ShoppingContext>(options =>
//    options.UseInMemoryDatabase("ShoppingDB"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Seed the database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ShoppingContext>();
    SeedData(context);
}

app.Run();

void SeedData(ShoppingContext context)
{
    if (!context.Products.Any())
    {
        context.Products.AddRange(
            new Product { Name = "Product 1", Price = 10.99m },
            new Product { Name = "Product 2", Price = 20.99m },
            new Product { Name = "Product 3", Price = 30.99m }
        );
        context.SaveChanges();
    }
}