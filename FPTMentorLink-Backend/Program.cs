using FPTMentorLink_Backend;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureDatabase();
builder.ConfigureJwt();
builder.ConfigureServices();
builder.ConfigureCors();
builder.ApplyMigration();

var app = builder.Build();

app.Configure();

app.Run();