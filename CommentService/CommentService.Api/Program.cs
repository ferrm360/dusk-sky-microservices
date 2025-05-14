using CommentService.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var mongoSettings = builder.Configuration.GetSection("MongoSettings");
string? connectionString = mongoSettings.GetValue<string?>("ConnectionString");
string? databaseName = mongoSettings.GetValue<string?>("DatabaseName");

if (string.IsNullOrWhiteSpace(connectionString) || string.IsNullOrWhiteSpace(databaseName))
{
    throw new InvalidOperationException("MongoSettings.ConnectionString o DatabaseName no están configurados.");
}

var mongoConnector = new MongoConnector();
var database = await mongoConnector.ConnectWithRetriesAsync(connectionString, databaseName);

builder.Services.AddSingleton(database);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", () => "Hello World!");

app.Run();
