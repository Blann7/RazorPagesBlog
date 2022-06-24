using Microsoft.EntityFrameworkCore;
using BlogPhone.BackgroundServices;
using Microsoft.AspNetCore.Authentication.Cookies;
using BlogPhone.Models.Database;
using BlogPhone.Models.LogViewer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Add LogViewer service
builder.Services.AddSingleton<DbLogger>();

// Main application database ----------------------------------------------------------------------
builder.Services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DB_Host_dev"))); // RegRu
// ------------------------------------------------------------------------------------------------

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options => 
    {
        options.LoginPath = "/auth/login";
        options.LogoutPath = "/auth/logout";
        options.Cookie.Name = "auth";
    });
builder.Services.AddAuthorization();

// IHosted services -------------------------------------------------------------------------------
builder.Services.AddHostedService<ValidateChecker>();
builder.Services.AddHostedService<LogSaver>();
// ------------------------------------------------------------------------------------------------

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

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
