using Microsoft.AspNetCore.Mvc.RazorPages;

namespace STHT.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    //private readonly UserShippingDbContext _dbContext;

    //include parameter UserShippingDbContext dbContext 
    //IHttpClientFactory httpClientFactory
    //Dependency injection
    public IndexModel(IHttpClientFactory httpClientFactory, ILogger<IndexModel> logger)
    {
       //_httpClientFactory = httpClientFactory;
        //_dbContext = dbContext;
        _logger = logger;
        _httpClientFactory = httpClientFactory;

    }

    public string? UserLocaleCountry { get; set; } 
    public string? ShippingApiResponse { get; set; } 

    //async Task
    public void  OnGet()
    {
        //await GetUserLocale();
        //await ShippingApiCall();
        UserLocaleCountry = "FR";

        //ProcessShippingData();
    }

   // API Call to make user locale
    //private async Task GetUserLocale()
    //{
        //UserLocaleCountry = "FR";
    //}
    //Api call to make external API call
    
    private async Task ShippingApiCall()
    {
        var client = _httpClientFactory.CreateClient("ShippingDataClient");
        //var bearerToken = "20231207160500";
        //client.DefaultRequestHeaders.Add("Authorization", $"Bearer {bearerToken}");
        //var response = await client.GetAsync("https://new-api.staging.spectinga.com/STHT/shipping");
        var response = await client.GetAsync("https://cat-fact.herokuapp.com/facts/");

        if (response.IsSuccessStatusCode)
        {
            ShippingApiResponse = await response.Content.ReadAsStringAsync();
            _logger.LogInformation(response.ReasonPhrase);
        }
        else
        {
            ShippingApiResponse = $"Error: {response.StatusCode}";
            _logger.LogCritical(response.ReasonPhrase);
        }
        
    }
    
    //Method to save Shipping Data in the database
    private void ProcessShippingData()
    {
        throw new NotImplementedException();
    }


    
    
}

