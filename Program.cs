using BlockedCountriesApi.Middleware;
using BlockedCountriesApi.Repositories;
using BlockedCountriesApi.Services;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Setup HTTP Client for IP API
builder.Services.AddHttpClient("IpApi", client =>
{
    // Configure default headers
    client.DefaultRequestHeaders.Add("User-Agent", "BlockedCountriesApi/1.0");
    // Free tier rate limits exist, but this satisfies basic setup
});

// Register Repositories as Singletons for In-Memory Storage
builder.Services.AddSingleton<IBlockedCountryRepository, BlockedCountryRepository>();
builder.Services.AddSingleton<ITemporalBlockRepository, TemporalBlockRepository>();
builder.Services.AddSingleton<IBlockAttemptLogRepository, BlockAttemptLogRepository>();

// Register Services
builder.Services.AddScoped<IIpGeolocationService, IpGeolocationService>();
builder.Services.AddScoped<ICountryService, CountryService>();
builder.Services.AddScoped<IBlockAttemptService, BlockAttemptService>();

// Register Background Service
builder.Services.AddHostedService<TemporalBlockCleanupService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => 
{
    c.SwaggerDoc("v1", new() { Title = "Blocked Countries API", Version = "v1" });
    
    // Set the comments path for the Swagger JSON and UI.
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if(File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// Configure Forwarded Headers to get client IP correctly behind proxies
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    // Clearing known networks ensures it accepts headers from any proxy (useful for testing, tighten in prod)
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseForwardedHeaders();

// Use our custom global exception handler
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
