using Concert_Ticket_Management_System.DataAccessLayer.Repos.EventRepository;
using Concert_Ticket_Management_System.DataAccessLayer.Repos.ReservationRepository;
using Concert_Ticket_Management_System.Services.ConcertServices;
using Concert_Ticket_Management_System.Shared.Mappers;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));


// Dependency Injection for my services
builder.Services.AddSingleton<IConcertService, ConcertService>();
builder.Services.AddSingleton<IConcertRepository, ConcertRepository>();
builder.Services.AddSingleton<IReservationRepository, ReservationRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
