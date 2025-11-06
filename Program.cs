using Microsoft.EntityFrameworkCore;
using RealEstateMVC.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Conditionally register RealEstateContext
if (builder.Environment.IsEnvironment("Testing"))
{
    // Use InMemory provider for integration tests
    builder.Services.AddDbContext<RealEstateContext>(options =>
        options.UseInMemoryDatabase("IntegrationDb"));
}
else
{
    // Use SQL Server provider for normal development/production
    builder.Services.AddDbContext<RealEstateContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();

// Needed for WebApplicationFactory<T> in integration tests
public partial class Program { }
