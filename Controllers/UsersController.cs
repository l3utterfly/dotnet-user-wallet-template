namespace WebApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebApi.Authorization;
using WebApi.Entities;
using WebApi.Helpers;
using WebApi.Models.Users;
using WebApi.Services;

[Authorize]
[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private UserService _userService;
    private readonly AppSettings _appSettings;
    private readonly DataContext _dataContext;
    private readonly PopulatorService _populatorService;

    public UsersController(
        UserService userService,
        IOptions<AppSettings> appSettings,
        DataContext dataContext,
        PopulatorService populatorService)
    {
        _userService = userService;
        _appSettings = appSettings.Value;
        _dataContext = dataContext;
        _populatorService = populatorService;
    }

    [AllowAnonymous]
    [HttpPost("authenticate")]
    public IActionResult Authenticate(AuthenticateRequest model)
    {
        var response = _userService.Authenticate(model);
        return Ok(response);
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public IActionResult Register(RegisterRequest model)
    {
        _userService.Register(model);
        return Ok(new { message = "Registration successful" });
    }

    [HttpGet("basicinfo")]
    public BasicInfoResponse GetBasicInfo()
    {
        var user = (User)HttpContext.Items["User"];

        return _populatorService.PopulateBasicInfoResponse(user);
    }
}