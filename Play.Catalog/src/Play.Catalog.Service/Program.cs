using MassTransit;
using Play.Catalog.Service.Entities;
using Play.Catalog.Service.Options;
using Play.Common.MongoDb;
using Play.Common.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json");

builder.Services.Configure<ServiceOptions>(builder.Configuration.GetSection(ServiceOptions.ServiceSettings));
var serviceSettings = builder.Configuration.GetSection(nameof(ServiceOptions)).Get<ServiceOptions>();

builder.Services.Configure<MongoDbOptions>(builder.Configuration.GetSection(MongoDbOptions.MongoDbSettings));

builder.Services.AddMongo()
    .AddMongoRepository<Item>("items");

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, configurator) =>
    {
        var rabbitMqOptions = builder.Configuration.GetSection(nameof(RabbitMQOptions)).Get<RabbitMQOptions>();
        configurator.Host(rabbitMqOptions.Host);
        configurator.ConfigureEndpoints(context, new KebabCaseEndpointNameFormatter(serviceSettings.ServiceName, false));
    });
});

// Add services to the container.
builder.Services.AddControllers(options =>
{
    options.SuppressAsyncSuffixInActionNames = false;
}
);
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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
