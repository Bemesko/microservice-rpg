using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Play.Catalog.Service.Options;
using Play.Catalog.Service.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json");

builder.Services.Configure<ServiceOptions>(builder.Configuration.GetSection(ServiceOptions.ServiceSettings));
builder.Services.Configure<MongoDbOptions>(builder.Configuration.GetSection(MongoDbOptions.MongoDbSettings));

builder.Services.AddSingleton(serviceProvider =>
{
    var serviceOptions = builder.Configuration.GetSection(ServiceOptions.ServiceSettings).Get<ServiceOptions>();
    var mongoOptions = builder.Configuration.GetSection(MongoDbOptions.MongoDbSettings).Get<MongoDbOptions>();
    var mongoClient = new MongoClient(mongoOptions.ConnectionString);
    return mongoClient.GetDatabase(serviceOptions.ServiceName);
});

builder.Services.AddSingleton<IItemsRepository, ItemsRepository>();

// Add services to the container.
builder.Services.AddControllers(options =>
{
    options.SuppressAsyncSuffixInActionNames = false;
}
);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));

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
