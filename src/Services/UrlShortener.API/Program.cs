using MassTransit;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Persistence;
using UrlShortener.Persistence.Repositories;
using UrlShortener.Shared;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddSingleton<Base62Encoder>();
builder.Services.AddScoped<IShortUrlRepository, ShortUrlRepository>();

var redisConn = builder.Configuration.GetConnectionString("Redis");
var instanceName = builder.Configuration["RedisOptions:InstanceName"];

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redisConn;
    options.InstanceName = instanceName ?? "Default_";
});

// RabbitMQ
var rabbitUserName = builder.Configuration["RabbitMQOptions:UserName"] ?? "guest";
var rabbitPassword = builder.Configuration["RabbitMQOptions:Password"] ?? "guest";
builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username(rabbitUserName);
            h.Password(rabbitPassword);
        });
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<DatabaseContext>();

        await context.Database.MigrateAsync();

        Console.WriteLine("The migrations were successfully applied.");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Migration application error");
    }
}

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
