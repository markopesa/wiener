using Wiener.Data.Interfaces;
using Wiener.Data.Repositories;
using Wiener.Data.Seed;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IPartnerRepository, PartnerRepository>();
builder.Services.AddScoped<IPolicyRepository, PolicyRepository>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var configuration = scope.ServiceProvider
        .GetRequiredService<IConfiguration>();
    var seeder = new DatabaseSeeder(configuration);
    await seeder.SeedAsync();
}


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Partners}/{action=Index}/{id?}");

app.Run();
