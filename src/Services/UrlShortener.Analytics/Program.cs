using MassTransit;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Analytics;
using UrlShortener.Persistence;
using UrlShortener.Persistence.Repositories;

var builder = Host.CreateApplicationBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<ILinkStatsRepository, LinkStatsRepository>();

var rabbitUserName = builder.Configuration["RabbitMQOptions:UserName"] ?? "guest";
var rabbitPassword = builder.Configuration["RabbitMQOptions:Password"] ?? "guest";
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<UrlClickedConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username(rabbitUserName);
            h.Password(rabbitPassword);
        });

        cfg.ConfigureEndpoints(context);
    });
});

var host = builder.Build();
host.Run();
