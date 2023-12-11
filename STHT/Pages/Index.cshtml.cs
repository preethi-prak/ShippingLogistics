using Microsoft.AspNetCore.Mvc.RazorPages;

namespace STHT.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    //private readonly IHttpClientFactory _httpClientFactory;
    //private readonly UserShippingDbContext _dbContext;

    //include parameter UserShippingDbContext dbContext 
    //IHttpClientFactory httpClientFactory
    //Dependency injection
    public IndexModel( ILogger<IndexModel> logger)
    {
       //_httpClientFactory = httpClientFactory;
        //_dbContext = dbContext;
        _logger = logger;
    }
    public string UserLocaleCountry { get; set; }
    public string ApiResponse { get; set; }

    //async Task
    public void OnGet()
    {
        //await GetUserLocale();
        //await MakeApiCall();
        //ProcessShippingData();
    }

   // API Call to make user locale
    private async Task GetUserLocale()
    {
        throw new NotImplementedException();
    }
    //Api call to make external API call
    
    private async Task MakeApiCall()
    {
        throw new NotImplementedException();
    }
    
    //Method to save Shipping Data in the database
    private void ProcessShippingData()
    {
        throw new NotImplementedException();
    }


    
    
}

