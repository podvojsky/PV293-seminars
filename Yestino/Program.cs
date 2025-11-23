using JasperFx;
using Wolverine;
using Wolverine.Http;
using Yestino.Order;
using Yestino.ProductCatalog;
using Yestino.Warehouse;
using Yestino.Wolverine;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.SetupWolverine();

builder.AddProductCatalogModule();
builder.AddWarehouseModule();
builder.AddOrderModule();
// TODO: register modules here

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

await app.Services.ApplyAsyncWolverineExtensions();
app.MapWolverineEndpoints();

app.UseHttpsRedirection();

return await app.RunJasperFxCommands(args.Length == 0 ? ["run"] : args);
