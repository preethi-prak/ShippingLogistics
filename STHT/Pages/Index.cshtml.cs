using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
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
        public int UserId { get; set; } = 1234;
        public int productId { get; set; }=  112 ;
        
        
        public Dictionary<string, decimal>? ShippingData { get; private set; }

        public async Task OnGetAsync()
        {
            
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
            
            UserLocaleCountry =  GetCountryCode();
            
        }
        private string GetCountryCode()
        {
            //Database access to get country code 
            return "FR";

        }

        private async Task<Dictionary<string, decimal>?> ShippingApiCallAsync()
        {
            var httpClient = _httpClientFactory.CreateClient("ShippingDataClient");
            var BaseUrl = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ShippingApi")["BaseUrl"];
            var bearerToken = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build()
                .GetSection("ShippingApi")["BearerToken"];

            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {bearerToken}");
            httpClient.DefaultRequestHeaders.Add("User-Agent", "MyAspNetCoreApp/6.0");

            try
            {
                //include URL and bearer token in appsetting.json - !IMPORTANT
                var response = await httpClient.GetAsync(BaseUrl);

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



        private void ProcessShippingData()
        {
            
        }
        
    }
}

