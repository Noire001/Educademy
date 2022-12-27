using Microsoft.AspNetCore.Mvc;

namespace WebStatus.Controllers;

public class HomeController : Controller
{
    private readonly IConfiguration _configuration;

    public HomeController(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    [HttpGet("/")]
    public IActionResult Index()
    {
        var basePath = _configuration["PATH_BASE"];
        return Redirect($"{basePath}/hc-ui");
    }
}