using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using STHT.Data; // Newtonsoft.Json for JSON
using STHT.Data.Models; 

namespace STHT.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ShippingDbContext _dbContext;
        
        // boolean val to check DB connection - intial test
        public bool IsDbConnected { get; set; }
        public Dictionary<string, decimal>? ShippingData { get; private set; }
        public string? ApiCountry { get; private set; }
        
        [BindProperty]
        public Shipping? NewShipping { get; set; }
        public Shipping? ExistingShipping { get; set; }
        //Using DI 
        public IndexModel(IHttpClientFactory httpClientFactory, ILogger<IndexModel> logger,ShippingDbContext dbContext)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _dbContext = dbContext;
        }

        //GET METHOD
        public async Task<IActionResult> OnGetAsync()
        {
            
            await GetCountryCode();
            ShippingData = await ShippingApiCallAsync();
            NewShipping = new Shipping()
            {
                UserId = 1122,
                ProductId = 111,
            }; 
                
            //update button 
            // record of userid and product id exists : get the values and reload . 
            // if not create new shipping 
            
            //await TestConnection();
            bool isDbConnected =  _dbContext.Database.CanConnect();
            if (!isDbConnected)
            {
                // Handle the error, maybe set an error message or log it
                ModelState.AddModelError(string.Empty, "Unable to connect to the database.");
                return RedirectToPage("/Error");
            }
            else
            {
                ExistingShipping = await _dbContext.ShippingDetails
                    .FirstOrDefaultAsync(s => s.UserId == NewShipping.UserId && s.ProductId == NewShipping.ProductId);

                if (ExistingShipping == null)
                {
                    //If there exist no record for the userid and password create new shipping 
                    NewShipping.CountryLocale = "FR";
                    NewShipping.ShippingCost = ProcessShippingData(NewShipping.CountryLocale, ShippingData);
                    NewShipping.OwnTransport = 0;
                    NewShipping.BidPrice = 2400;
                    NewShipping.DeliveryOption = "OwnTransport";

                    if (NewShipping.DeliveryOption == "DeliveryToYard")
                        NewShipping.TotalPrice = NewShipping.ShippingCost + NewShipping.BidPrice;
                    else
                        NewShipping.TotalPrice = NewShipping.BidPrice;
                }
                else
                {
                    //if record exists for the user - assign the values in database
                    NewShipping = ExistingShipping;
                }

                return Page();
            }

        }
        
        // POSTMETHOD on Update shipping modal 
        public IActionResult OnPostUpdateShipping()
        {
            

           if (NewShipping != null || ModelState.IsValid )
           {
               bool isDbConnected =  _dbContext.Database.CanConnect();
               if (!isDbConnected)
               {
                   // Handle the error, maybe set an error message or log it
                   ModelState.AddModelError(string.Empty, "Unable to connect to the database.");
                   return RedirectToPage("/Error");
               }
               if (NewShipping.DeliveryOption != null)
               {
                   _dbContext.CreateOrUpdateShipping(NewShipping);
                    
               }
           
               //Call Get method on index page load
               return RedirectToPage();
           }
           else
           {
               ModelState.AddModelError(string.Empty, "Shipping Data is null or Model state is invalid , refill form and submit");
               return RedirectToPage("/Error");
           }
           
        }
        public  IActionResult OnPostBidding()    
        {

            if (NewShipping != null || ModelState.IsValid  )
            {
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
                        _dbContext.CreateOrUpdateShippingForBidding(NewShipping);
                    
                    }else if (NewShipping?.DeliveryOption == "OwnTransport")
                    {
                        NewShipping.TotalPrice = NewShipping.BidPrice;
                        _dbContext.CreateOrUpdateShippingForBidding(NewShipping);
                    }

                    // Redirect to a success page because the submission is done
                
                    return RedirectToPage("/Success");
                
                }
               
            }
            else
            {
                return RedirectToPage("/Error");
                
            }

            

            
        }

        //  Minimal API - call to get locale user
        // culture info is razor app is automatically set to accept headers of http request
        // not always accurate
        // localhost:8080/cultureinfo
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
                    var shippingData = JsonConvert.DeserializeObject<Dictionary<string, decimal>?>(shippingResponse);
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

        private decimal ProcessShippingData(string? userLocale,Dictionary<string, decimal>? shipData)
        {
            if (userLocale != null && shipData != null && shipData.ContainsKey(userLocale))
            {
                decimal shipping = shipData[userLocale];
                return shipping;
            }
            else
            {
                return 0;
            }
        }
        
        // Code to test Database connection - Successful
        private async Task TestConnection()
        {
            try
            {
                // Check if the database connection can be established
                IsDbConnected = await _dbContext.Database.CanConnectAsync();
            }
            catch (Exception ex)
            {
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
