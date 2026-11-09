using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using Microsoft.AspNetCore.Http;
using RaceDay.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
if (!string.IsNullOrWhiteSpace(builder.Configuration["AzureBlobStorage:ConnectionString"]))
    builder.Services.AddSingleton<BlobStorageService>();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.IdleTimeout = TimeSpan.FromMinutes(30);
});
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<RaceDayDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddOpenApi();

var app = builder.Build();

// Ensure the OpenAPI/Swagger JSON is generated at the expected path and the UI can find it.
app.UseSwagger();

// Serve Swagger UI at application root so the browser opened by launchSettings goes to Swagger.
// Explicitly point to the generated swagger.json to avoid path mismatches when other OpenAPI packages are present.
app.UseSwaggerUI(c =>
{
    c.RoutePrefix = string.Empty;
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "RaceDay API V1");
});

// If using MapOpenApi from Minimal APIs or another openapi package, ensure it's mapped in Development.
if (app.Environment.IsDevelopment())
{
    try
    {
        app.MapOpenApi();
    }
    catch (Exception)
    {
        // If MapOpenApi is not available or fails, ignore so SwaggerGen still provides the JSON.
    }
}

app.UseHttpsRedirection();

app.UseSession();
app.UseAuthorization();

app.MapControllers();

app.Run();