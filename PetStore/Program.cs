using Microsoft.EntityFrameworkCore;
using PetStore.Models;
using PetStore.Pages.Customer;

var builder = WebApplication.CreateBuilder(args);
// Đăng ký EmailService
builder.Services.AddTransient<Verify_Profile_Update>();
builder.Services.AddDbContext<PetStoreContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("value")));

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddDbContext<PetStoreContext>();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromDays(1);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthorization();
app.MapGet("/", context =>
{
    context.Response.Redirect("/Home");
    return Task.CompletedTask;
});
app.MapRazorPages();

app.Run();