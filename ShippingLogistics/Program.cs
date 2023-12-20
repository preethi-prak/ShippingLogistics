using Microsoft.EntityFrameworkCore;
using ShippingLogistics.Data;

// Create a builder for the web application
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//Register to DI container  to enable Razor pages
builder.Services.AddRazorPages();
//Service to register  and use IHttpClientFactory
builder.Services.AddHttpClient();
//Service to register DBContext Class. 
builder.Services.AddDbContext<ShippingDbContext>(option =>
    option.UseNpgsql(builder.Configuration.GetConnectionString("ConnString")));

var app = builder.Build();

//if the env isn't Development we have to display a friendly way to show expception
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
// If there is a request for stati files , we can serve it before the entire file loads 
app.UseStaticFiles();
//page routing
app.UseRouting();

app.UseAuthorization();
//rendering and routing of razer pages 
//map razor pages with routes OnGet - GET , OnPost - POST
app.MapRazorPages();
//Minimal API for cultureinfo - Change URL to getUserLocale!! 
app.MapGet("/getUserLocale", (HttpContext context) =>
{
    var cultureInfo = System.Globalization.CultureInfo.CurrentCulture;
    //en-US
    var cultureParts = cultureInfo.Name.Split('-');
    var countryCode = cultureParts.Length > 1 ? cultureParts[1] : "Unknown";
    return Results.Ok(new
    {
        country = countryCode
    });
});

app.Run();