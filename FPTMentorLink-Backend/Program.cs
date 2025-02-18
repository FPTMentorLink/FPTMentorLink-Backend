using System.Reflection;
using FPTMentorLink_Backend;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddUserSecrets(Assembly.GetExecutingAssembly(), true);
builder.ConfigureDatabase();
builder.ConfigureJwt();
builder.ConfigureServices();
builder.ConfigureCors();
builder.ApplyMigration();

builder.Services.AddControllers();
builder.ConfigureSwagger();
builder.Services.AddHealthChecks();

var app = builder.Build();

app.Configure();

app.Run();