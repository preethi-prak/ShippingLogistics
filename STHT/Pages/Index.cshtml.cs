using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
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
                    UserId = 1122,
                    ProductId = 111,
                    CountryLocale = UserLocaleCountry,
                    ShippingCost = ProcessShippingData(UserLocaleCountry),
                    OwnTransport = 0,
                    BidPrice = 2400,
                    DeliveryOption ="OwnTransport"
                
                };
            //also check what delivaryoption is set to ? - IMP
                if (NewShipping.DeliveryOption =="DeliveryToYard")
                {
                    NewShipping.TotalPrice = NewShipping.ShippingCost + NewShipping.BidPrice;
                }
                else
                {
                    NewShipping.TotalPrice = NewShipping.BidPrice;
                }
                
            //update button 
            // record of userid and product id exists : 
            //get the values and reload . 
            // if not leave intial values
            
            //await TestConnection();
 
        }
        
        public IActionResult OnPostUpdateShipping()
        {
            
           var sh_userid = NewShipping.UserId;
           var sh_prodid = NewShipping.ProductId;
           var del = NewShipping.DeliveryOption;
           var price = NewShipping.ShippingCost;
           var c = NewShipping.CountryLocale;
           var aa = NewShipping.OwnTransport;
           
           bool isDbConnected =  _dbContext.Database.CanConnect();
           
           if (!ModelState.IsValid)
           {
               return RedirectToPage("/Error");
           }
           
          
           if (!isDbConnected)
           {
               // Handle the error, maybe set an error message or log it
               ModelState.AddModelError(string.Empty, "Unable to connect to the database.");
               return RedirectToPage("/Error");
           }
           
           //Call Get method on index page load
            return RedirectToPage();
        }
        public  IActionResult OnPostBidding()    
        {
            var sh_userid = NewShipping.UserId;
            var sh_prodid = NewShipping.ProductId;
            var del = NewShipping.DeliveryOption;
            var price = NewShipping.ShippingCost;
            var c = NewShipping.CountryLocale;
            var aa = NewShipping.OwnTransport;
            var bid = NewShipping.BidPrice;
            
            if (!ModelState.IsValid)
            {
                return RedirectToPage("/Error");
            }

            // Check database connection
            bool isDbConnected =  _dbContext.Database.CanConnect();
            if (!isDbConnected)
            {
                // Handle the error, maybe set an error message or log it
                ModelState.AddModelError(string.Empty, "Unable to connect to the database.");
                return RedirectToPage("/Error");
            }
            else
            {
                if (NewShipping.DeliveryOption == "DeliveryToYard")
                {
                    NewShipping.TotalPrice = NewShipping.ShippingCost + NewShipping.BidPrice;
                    _dbContext.CreateOrUpdateShipping(NewShipping);
                    
                }else if (NewShipping.DeliveryOption == "OwnTransport")
                {
                    NewShipping.TotalPrice = NewShipping.BidPrice;
                    _dbContext.CreateOrUpdateShipping(NewShipping);
                }
                // Proceed with create or update operation
                

                // Redirect to a success page because the submission is done
                
                return RedirectToPage("/Success");
                
            }

            
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
