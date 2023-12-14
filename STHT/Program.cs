using Microsoft.EntityFrameworkCore;
using STHT.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
//Service to register  and use IHttpClientFactory
builder.Services.AddHttpClient();
//Service to register DBContext Class. 
builder.Services.AddDbContext<ShippingDbContext>(option =>
    option.UseNpgsql(builder.Configuration.GetConnectionString("ConnString")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();
// Define the minimal API endpoint
app.MapGet("/cultureinfo", (HttpContext context) => 
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