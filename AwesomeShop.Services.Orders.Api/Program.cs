using AwesomeShop.Services.Orders.Application;
using AwesomeShop.Services.Orders.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHandlers();
builder.Services.AddMongo();
builder.Services.AddRepositories();
builder.Services.AddHttpClient();
builder.Services.AddMessageBus();
builder.Services.AddSubscribers();
builder.Services.AddConsultConfig(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseConsul();

app.UseAuthorization();

app.MapControllers();

app.Run();
