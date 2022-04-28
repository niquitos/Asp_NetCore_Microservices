using System;
using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Play.Common.MassTransit;
using Play.Common.MongoDB;
using Play.Inventory.Service.Clients;
using Play.Inventory.Service.Entities;
using Polly;
using Polly.Timeout;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddMongo()
                .AddMongoRepository<InventoryItem>("inventoryitems")
                .AddMongoRepository<CatalogItem>("catalogitems")
                .AddMassTransitWithRabbitMQ();

var jitterer = new Random();

AddCatalogClient(builder, jitterer);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

static void AddCatalogClient(WebApplicationBuilder builder, Random jitterer)
{
    builder.Services.AddHttpClient<CatalogClient>(client =>
    {
        client.BaseAddress = new Uri("https://localhost:7202");
    })
    .AddTransientHttpErrorPolicy(
        builder => builder.Or<TimeoutRejectedException>().WaitAndRetryAsync(
            5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)) + TimeSpan.FromMilliseconds(jitterer.Next(0, 1000))))
    .AddTransientHttpErrorPolicy(builder => builder.Or<TimeoutRejectedException>().CircuitBreakerAsync(
        3,
        TimeSpan.FromSeconds(15),
        onBreak: (outcome, timespan) =>
        {
            Console.WriteLine($"Opening the circuit for {timespan.TotalSeconds} seconds...");
        },
        onReset: () =>
        {
            Console.WriteLine("Closing the circuit");
        }))
    .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(1));
}