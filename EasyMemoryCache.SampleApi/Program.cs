using EasyMemoryCache.Configuration;

var builder = WebApplication.CreateBuilder(args);

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", true, false)
    .AddEnvironmentVariables()
    .Build();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var cacheProvider =
    (CacheProvider)Enum.Parse(typeof(CacheProvider), configuration.GetSection("CacheSettings:CacheProvider").Value);

builder.Services.AddEasyCache(new CacheSettings()
{
    // Redis Configuration
    RedisConnectionString = configuration.GetSection("CacheSettings:RedisConnectionString").Value,

    // Provider Selection
    CacheProvider = cacheProvider
}, configuration.GetSection("CacheSettings"));

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