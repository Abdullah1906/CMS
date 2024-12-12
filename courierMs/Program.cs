using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using courierMs.Data;
using courierMs.Areas.Identity.Model;
using courierMs.DataModel;
var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("courierMsContextConnection") ?? throw new InvalidOperationException("Connection string 'courierMsContextConnection' not found.");

builder.Services.AddDbContext<courierMsContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<courierMsContext>();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<mycontext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("courierMsContextConnection")));
builder.Services.AddSession();

var app = builder.Build();

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
app.UseSession();
app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");



app.Run();
