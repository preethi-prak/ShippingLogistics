using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json; //  Newtonsoft.Json for JSON 


namespace STHT.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public IndexModel(IHttpClientFactory httpClientFactory, ILogger<IndexModel> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public string? UserLocaleCountry { get; set; } 
        public string? ShippingApiResponse { get; set; }
        public Dictionary<string, decimal>? ShippingData { get; private set; }

        public async Task OnGetAsync()
        {
            // await GetUserLocale();
            UserLocaleCountry = "FR"; 
            ShippingData = await ShippingApiCallAsync();
            // ProcessShippingData();
            
            if (ShippingData is not null)
            {
                foreach(var item in ShippingData)
                {
                    Console.Write(item.Key);
                    Console.Write(item.Value);
                    Console.WriteLine("/n");
                
                }
                
            }
            
        }

        private async Task<Dictionary<string, decimal>?> ShippingApiCallAsync()
        {
            var httpClient = _httpClientFactory.CreateClient("ShippingDataClient");
            var bearerToken = "20231207160500";

            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {bearerToken}");
            httpClient.DefaultRequestHeaders.Add("User-Agent", "MyAspNetCoreApp/6.0");

            try
            {
                //include URL and bearer token in appsetting.json - !IMPORTANT
                var response = await httpClient.GetAsync("https://new-api.staging.spectinga.com/STHT/shipping");

                if (response.IsSuccessStatusCode)
                {
                    var shippingResponse = await response.Content.ReadAsStringAsync();
                    var shippingData = JsonConvert.DeserializeObject<Dictionary<string, decimal>>(shippingResponse);
                    _logger.LogInformation(" API call Success");
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
                _logger.LogCritical($"Exception : {ex.Message}");
                return null;
            }
        }

        /*
        private async Task GetUserLocale()
        {
            // Implementation here.
        }

        private void ProcessShippingData()
        {
            // Implementation here.
        }
        */
    }
}
