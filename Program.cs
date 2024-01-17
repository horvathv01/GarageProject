using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using GarageProject.Auth;
using GarageProject.DAL;
using GarageProject.Models;
using GarageProject.Service;
using GarageProject.Service.Factories;
using GarageProject.Service;
using GarageProject.Converters;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.WithOrigins("http://localhost:3000", "http://192.168.4.80:3000")
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


// Add services to the container.
builder.Services.AddDbContext<GarageProjectContext>(options => 
    options.UseNpgsql(builder.Configuration.GetConnectionString( "GarageProjectConnection" ) ));


builder.Services.AddScoped<IAccessUtilities, AccessUtilities>();
builder.Services.AddScoped<IHasherFactory, HasherFactory>();

builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IManagerService, ManagerService>();
builder.Services.AddTransient<IBookingService, BookingService>();
builder.Services.AddTransient<IParkingSpaceService, ParkingSpaceService>();

//prepopulate DB and/or in memory repositories via interface for testing purposes
//builder.Services.AddTransient<IPrepopulate, Prepopulate>();
builder.Services.AddScoped<IDateTimeConverter, DateTimeConverter>();

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
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