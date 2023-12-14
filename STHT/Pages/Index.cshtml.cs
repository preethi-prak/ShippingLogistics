using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json; // Newtonsoft.Json for JSON


namespace STHT.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public string? UserLocaleCountry { get; set; } 
        public string? ShippingApiResponse { get; set; }
        public int UserId { get; set; } = 1234;
        public int ProductId { get; set; } = 112;
        public float ShippingCost { get; set; } 
        public Dictionary<string, float>? ShippingData { get; private set; }
        public string? DeliveryOption { get; set; }
        public string? Country { get; private set; }
        
        public IndexModel(IHttpClientFactory httpClientFactory, ILogger<IndexModel> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task OnGetAsync()
        {
            await GetCountryCode();
            ShippingData = await ShippingApiCallAsync();
            UserLocaleCountry = "FR";
            ShippingCost = ProcessShippingData(UserLocaleCountry);
            //_logger.LogInformation($"Shipping Cost: {ShippingCost}");
        }

        public IActionResult OnPostUpdateShipping()
        {
           // _logger.LogWarning($"Received data: {JsonConvert.SerializeObject(Request.Form)}");
           foreach (var key in Request.Form.Keys)
           {
               _logger.LogCritical($"{key}: {Request.Form[key]}");
           }

           _logger.LogCritical("Inside Update Shipping");
            return RedirectToPage("/Success");
        }

        public IActionResult OnPostBidding()
        {
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
                    var Info = JsonConvert.DeserializeObject<dynamic>(apiResponse);
                    Country = Info?.country;
                }
                else
                {
                    Country = "NONE";
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception: {ex.Message}");
                Country = "NONE";
                
            }
            
        }

        private async Task<Dictionary<string, float>?> ShippingApiCallAsync()
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
                    var shippingData = JsonConvert.DeserializeObject<Dictionary<string, float>>(shippingResponse);
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

        private float ProcessShippingData(string userLocale)
        {
            if (userLocale != null && ShippingData != null && ShippingData.ContainsKey(userLocale))
            {
                float shipping = ShippingData[userLocale];
                return shipping;
            }
            else
            {
                return 0;
            }
        }
    }
}
