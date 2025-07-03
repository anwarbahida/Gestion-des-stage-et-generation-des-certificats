using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using appWeb.Data;
using appWeb.Services;
using appWeb.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using QuestPDF.Infrastructure;
using System.Security.Principal;


// Déclare que tu utilises la licence Community
QuestPDF.Settings.License = LicenseType.Community;
var builder = WebApplication.CreateBuilder(args);


//builder.WebHost.UseUrls("http://192.168.11.184:5000");

// ✅ Ajouter les services AVANT builder.Build()
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<CertificatService>();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSingleton<IEmailSender, FakeEmailSender>();


// ✅ Ajouter Identity AVANT Build
builder.Services.AddIdentity<Admin, IdentityRole>(options => 
{
    options.SignIn.RequireConfirmedAccount = false;
    // Autres options si besoin
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddRazorPages(); // Important pour Identity UI

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // ✅ AVANT UseAuthorization
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages(); // ✅ Pour les pages de Login/Register si tu les uses

app.Run();
