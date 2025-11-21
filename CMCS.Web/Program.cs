using CMCS.Web.Data;
using CMCS.Web.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllersWithViews();
var conn = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=cmcs_web.db";
var dbFile = conn.Replace("Data Source=", "").Trim();

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(conn));
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<IClaimService, ClaimService>();

var app = builder.Build();

// Ensure DB and seed (fresh/dev-friendly)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();


    if (!db.Users.Any())
    {
        db.Users.AddRange(new[]
        {
            new CMCS.Web.Models.User { Username="lect1", FirstName="Lindi", LastName="Ndlovu", Email="lindi@example.com", Role="Lecturer"},
            new CMCS.Web.Models.User { Username="coord1", FirstName="Nomsa", LastName="Dlamini", Email="nomsa@example.com", Role="Coordinator"},
            new CMCS.Web.Models.User { Username="mgr1", FirstName="Sipho", LastName="Mthethwa", Email="sipho@example.com", Role="Manager"}
        });

        db.SaveChanges();
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
