using WebAutoTest.ApplicationOptions;
using WebAutoTest.Repositories;
using WebAutoTest.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


var configuration = builder.Configuration;
builder.Services.Configure<TicketSettings>(configuration.GetSection("TicketSettings"));


builder.Services.AddControllersWithViews();

builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<CookieService>();
builder.Services.AddScoped<QuestionsRepository>();
builder.Services.AddScoped<TicketsRepository>();
builder.Services.AddScoped<UsersRepository>();




var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
