using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using GarageProject.Auth;
using GarageProject.DAL;
using GarageProject.Models;
using GarageProject.Service;
using GarageProject.Service.Factories;
using GarageProject.Service;
using GarageProject.Converters;
using PsychAppointments_API.Converters;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", builder =>
    {
        builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
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
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAllOrigins");

//app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();