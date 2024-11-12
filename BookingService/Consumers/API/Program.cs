using Application.Ports;
using Application.Guest;
using Data;
using Data.Guest;
using Domain.Ports;
using Microsoft.EntityFrameworkCore;
using Application.Room;
using Application.Booking;
using Data.Room;
using Data.Booking;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

#region
builder.Services.AddScoped<IGuestManager, GuestManager>();
builder.Services.AddScoped<IGuestRepository, GuestRepository>();
builder.Services.AddScoped<IBookingManager, BookingManager>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IRoomManager, RoomManager>();
builder.Services.AddScoped<IRoomRepository, RoomRepository>();
#endregion

#region

var connectionString = builder.Configuration.GetConnectionString("Main");
builder.Services.AddDbContext<HotelDbContext>(
    options =>options.UseSqlServer(connectionString));

#endregion




// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Hotel Booking API",
        Version = "v1",
        Description = "API for managing hotel bookings, guests, and rooms",
        TermsOfService = new Uri("https://github.com/joyce-santos23"),
        Contact = new OpenApiContact
        {
            Name = "Joyce Santos Mendes",
            Email = "joycectba@hotmail.com",
            Url = new Uri("https://github.com/joyce-santos23")
        },
        License = new OpenApiLicense
        {
            Name = "Termo de Licença de Uso",
            Url = new Uri("https://github.com/joyce-santos23")
        }
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
