using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Npgsql.Replication;
using STHT.Data; // Newtonsoft.Json for JSON
using STHT.Data.Models; 

namespace STHT.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ShippingDbContext _dbContext;

        public bool IsDbConnected { get; set; }
        public string? UserLocaleCountry { get; set; } 
        public Dictionary<string, decimal>? ShippingData { get; private set; }
        public string? ApiCountry { get; private set; }
        [BindProperty]
        public Shipping NewShipping { get; set; }
        
        public IndexModel(IHttpClientFactory httpClientFactory, ILogger<IndexModel> logger,ShippingDbContext dbContext)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task OnGetAsync()
        {
            
            await GetCountryCode();
            ShippingData = await ShippingApiCallAsync();
            UserLocaleCountry = "FR";
            //ShippingCost = ProcessShippingData(UserLocaleCountry);
            NewShipping = new Shipping()
            {
                UserId = 1234,
                ProductId = 111,
                CountryLocale = UserLocaleCountry,
                ShippingCost = ProcessShippingData(UserLocaleCountry),
                OwnTransport = 0,
                BidPrice = 2400,
                
            };
            
            if (NewShipping.ShippingCost != 0)
            {
                NewShipping.TotalPrice = NewShipping.ShippingCost + NewShipping.BidPrice;
            }
            else
            {
                NewShipping.TotalPrice = NewShipping.BidPrice;
            }
            //await TestConnection();
            
           // _logger.LogInformation($"Data: {NewShipping}");
        }
        
        public IActionResult OnPostUpdateShipping()
        {
            
           var sh_userid = NewShipping.UserId;
           var sh_prodid = NewShipping.ProductId;
           var del = NewShipping.DeliveryOption;
           var price = NewShipping.ShippingCost;
           var c = NewShipping.CountryLocale;
           var aa = NewShipping.OwnTransport;
           var bid = NewShipping.BidPrice;
           var total = NewShipping.TotalPrice;
           
            return RedirectToPage("/Success");
        }
        public IActionResult OnPostBidding()    
        {
            var sh_userid = NewShipping.UserId;
            var sh_prodid = NewShipping.ProductId;
            var del = NewShipping.DeliveryOption;
            var price = NewShipping.ShippingCost;
            var c = NewShipping.CountryLocale;
            var aa = NewShipping.OwnTransport;
            var bid = NewShipping.BidPrice;
            var total = NewShipping.TotalPrice;
            return RedirectToPage("/Success");
        }

        // API call the Minimal API
        private async Task GetCountryCode()
        {
            var httpClient = _httpClientFactory.CreateClient("GetCountryCodeClient");
            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            var cultureInfoUrl = configuration.GetSection("CountryCodeApi")["Url"];
            try
            {
                var response = await httpClient.GetAsync(cultureInfoUrl);
                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    var info = JsonConvert.DeserializeObject<dynamic>(apiResponse);
                    ApiCountry = info?.country;
                }
                else
                {
                    ApiCountry = "NONE";
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception: {ex.Message}");
                ApiCountry = "NONE";
                
            }
            
        }

        private async Task<Dictionary<string, decimal>?> ShippingApiCallAsync()
        {
            var httpClient = _httpClientFactory.CreateClient("ShippingDataClient");
            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            var baseUrl = configuration.GetSection("ShippingApi")["BaseUrl"];
            var bearerToken = configuration.GetSection("ShippingApi")["BearerToken"];

            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {bearerToken}");
            httpClient.DefaultRequestHeaders.Add("User-Agent", "MyAspNetCoreApp/6.0");

            try
            {
                var response = await httpClient.GetAsync(baseUrl);

                if (response.IsSuccessStatusCode)
                {
                    var shippingResponse = await response.Content.ReadAsStringAsync();
                    var shippingData = JsonConvert.DeserializeObject<Dictionary<string, decimal>>(shippingResponse);
                    _logger.LogInformation("API call Success");
                    return shippingData;
                }
                else
                {
                    var error = $"Error: {response.StatusCode}";
                    _logger.LogError($"API call failed: {error}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception: {ex.Message}");
                return null;
            }
        }

        private decimal ProcessShippingData(string userLocale)
        {
            if (userLocale != null && ShippingData != null && ShippingData.ContainsKey(userLocale))
            {
                decimal shipping = ShippingData[userLocale];
                return shipping;
            }
            else
            {
                return 0;
            }
        }
        
        // Code to test Database connection - Sucessful
        private async Task TestConnection()
        {
            try
            {
                // Check if the database connection can be established
                IsDbConnected = await _dbContext.Database.CanConnectAsync();
                
            }
            catch (Exception ex)
            {
                // Log the exception details
                // For example, using ILogger
                IsDbConnected = false;
            }

            if (IsDbConnected)
            {
                _logger.LogInformation("Connected");
            }
            else
            {
                _logger.LogCritical("NOT CONNECTED");
                
            }
        }

    }
}
