using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Melpominee.app.Models.CharacterSheets;
namespace Melpominee.app.Controllers;

[ApiController]
[Route("[controller]/[action]")]
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public class SheetController : ControllerBase
{
    private readonly ILogger<SheetController> _logger;
    public SheetController(ILogger<SheetController> logger)
    {
        _logger = logger;
    }
}