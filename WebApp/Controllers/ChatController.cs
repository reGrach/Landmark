using Microsoft.AspNetCore.Mvc;

namespace Landmark.WebApp.Controllers;

public class ChatController : Controller
{
    private readonly ILogger<ChatController> _logger;

    public ChatController(ILogger<ChatController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }
}