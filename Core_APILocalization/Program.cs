using System.Reflection;
using Core_APILocalization.Database;
using Core_APILocalization.LocalizationService;
using Core_APILocalization.Models;
using Microsoft.AspNetCore.Localization;

var builder = WebApplication.CreateBuilder(args);

// Register the Localization service
builder.Services.AddLocalization(options =>
{
    options.ResourcesPath = "Resources";
});

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[]
    {
        "en-US","de"
    };
   options.SetDefaultCulture(supportedCultures[0])
          .AddSupportedCultures(supportedCultures)
          .AddSupportedUICultures(supportedCultures);
});

builder.Services.AddScoped<IAppLocalizerService, AppLocalizerService>();

builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.PropertyNameCaseInsensitive = false;
    options.SerializerOptions.PropertyNamingPolicy = null;
    options.SerializerOptions.WriteIndented = true;
});

// Add services to the container.
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Localization configuration
var cultures = new[] { "en", "de" };
var requestLocalizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture(cultures[0])
    .AddSupportedCultures(cultures)
    .AddSupportedUICultures(cultures);

requestLocalizationOptions.RequestCultureProviders.Insert(0, new AcceptLanguageHeaderRequestCultureProvider());
app.UseRequestLocalization(requestLocalizationOptions);
app.UseStaticFiles();

app.MapGet("/", (HttpContext ctx, ILogger<Program> logger) =>
{
    var acceptLanguage = ctx.Request.Headers["Accept-Language"];
    logger.LogInformation($"Accept-Language Header: {acceptLanguage}");
    return Results.Ok($"Accept-Language: {acceptLanguage}");
});

app.MapGet("/persons", (HttpContext ctx, IAppLocalizerService serv) =>
{
    var persons = new PersonDB();
    var response = new List<Dictionary<string, string>>();

    var properties = typeof(Person).GetProperties(BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance);

    foreach (var person in persons)
    {
        var record = properties.ToDictionary(
            property => serv.GetLocalizationString(property.Name),
            property => serv.GetLocalizationString(property.GetValue(person)?.ToString() ?? string.Empty)
        );

        response.Add(record);
    }

    return Results.Ok(response); 
})
.WithName("GetPersons");

app.MapGet("/persons/{id}", (HttpContext ctx, IAppLocalizerService serv, int id, ILogger<Program> logger) =>
{
    var acceptLanguage = ctx.Request.Headers["Accept-Language"];
    logger.LogInformation($"Accept-Language Header {id}: {acceptLanguage}");
    var persons = new PersonDB();
    var response = persons.Where(p => p.PersonId == id).FirstOrDefault();
    if (response == null)
    {
        return Results.NotFound();
    }

    // Read Ptoperties of the Person class
    var properties = response.GetType().GetProperties(BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance);

    var record = properties.ToDictionary(property => serv.GetLocalizationString(property.Name), property => serv.GetLocalizationString(property.GetValue(response)?.ToString() ?? string.Empty));

    return Results.Ok(record);
})
.WithName("GetPersonsById");

app.Run();
