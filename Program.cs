using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using GarageProject.Auth;
using GarageProject.DAL;
using GarageProject.Models;
using GarageProject.Service;
using GarageProject.Service.Factories;
using GarageProject.Converters;
using System.Net;
using System.Net.Sockets;
using PsychAppointments_API.Service;
using PsychAppointments_API.Models.Enums;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

var ip = IPManager.GetIpAddress();
var httpsURL = IPManager.GenerateURL( URLType.https, ip, "7021" );
var httpURL = IPManager.GenerateURL(URLType.http, ip, "5082");

var frontEndAddress = Environment.GetEnvironmentVariable( "frontendUrl" ) ?? $"http://{ip}:3000";

if ( Debugger.IsAttached )
{
    frontEndAddress = $"http://{ip}:3000"; //always set frontEndAddress to current machine's ip if run in debug mode
    builder.WebHost.UseUrls( httpsURL, httpURL );
}

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.WithOrigins("http://localhost:3000", frontEndAddress)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
});


builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        
        options.Events.OnRedirectToLogin = context =>
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        };
        
        options.Cookie.Name = "GarageProjectCookie";
        options.Cookie.SameSite = SameSiteMode.None;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        //works in https only:
        //options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    });

builder.Services.AddControllers().AddNewtonsoftJson();

builder.Services.AddDbContext<GarageProjectContext>(options => 
    options.UseNpgsql(builder.Configuration.GetConnectionString( "GarageProjectConnection" ) ));

builder.Services.AddSingleton<ILoggerService, LoggerService>();

builder.Services.AddScoped<IAccessUtilities, AccessUtilities>();
builder.Services.AddScoped<IHasherFactory, HasherFactory>();

builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IManagerService, ManagerService>();
builder.Services.AddTransient<IBookingService, BookingService>();
builder.Services.AddTransient<IParkingSpaceService, ParkingSpaceService>();

builder.Services.AddScoped<IDateTimeConverter, DateTimeConverter>();
builder.Services.AddScoped<IUserConverter, UserConverter>();
builder.Services.AddScoped<IBookingConverter, BookingConverter>();

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
if( Debugger.IsAttached )
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();

//app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();