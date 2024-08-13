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
builder.Services.AddDbContext<ShoppingContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"), new MySqlServerVersion(new Version(8, 0, 21))));

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalFile",
        builder => builder
            .WithOrigins("null") // 允許null來源
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

// Use CORS
app.UseCors("AllowLocalFile");

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
    context.Database.EnsureDeleted();
    context.Database.EnsureCreated();

    context.Products.AddRange(
        new Product { Name = "Product 1", Price = 10.99m, Quantity = 20 },
        new Product { Name = "Product 2", Price = 20.99m, Quantity = 15 },
        new Product { Name = "Product 3", Price = 30.99m, Quantity = 5 }
    );

    context.Users.AddRange(
        new User { Username = "admin", Password = Convert.ToBase64String(Encoding.UTF8.GetBytes("adminPass123!")), Bill = 0},
        new User { Username = "user1", Password = Convert.ToBase64String(Encoding.UTF8.GetBytes("userPass456!")), Bill = 0},
        new User { Username = "user2", Password = Convert.ToBase64String(Encoding.UTF8.GetBytes("userPass789!")), Bill = 0}
    );

    context.SaveChanges();
}
