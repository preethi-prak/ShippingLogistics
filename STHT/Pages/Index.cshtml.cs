using Microsoft.AspNetCore.Mvc.RazorPages;

namespace STHT.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    //private readonly UserShippingDbContext _dbContext;

    //include parameter UserShippingDbContext dbContext 
    //Dependency injection
    public IndexModel(IHttpClientFactory httpClientFactory, ILogger<IndexModel> logger)
    {
        _httpClientFactory = httpClientFactory;
        //_dbContext = dbContext;
        _logger = logger;
    }
    public string UserLocaleCountry { get; set; }
    public string ApiResponse { get; set; }

    public async Task OnGet()
    {
        await GetUserLocale();
        await MakeApiCall();
        ProcessShippingData();
    }

    
    private async Task GetUserLocale()
    {
        throw new NotImplementedException();
    }
    
    private async Task MakeApiCall()
    {
        throw new NotImplementedException();
    }
    
    private void ProcessShippingData()
    {
        throw new NotImplementedException();
    }
}

