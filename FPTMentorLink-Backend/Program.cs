using System.Reflection;
using FPTMentorLink_Backend;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureDatabase();
builder.ConfigureJwt();
builder.ConfigureGoogleAuth();
builder.ConfigureServices();
builder.ConfigureCors();
builder.ApplyMigration();

builder.Services.AddControllers();
builder.ConfigureSwagger();
builder.Services.AddHealthChecks();

var app = builder.Build();

app.Configure();

app.Run();