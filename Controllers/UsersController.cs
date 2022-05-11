namespace WebApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net;
using WebApi.Authorization;
using WebApi.Entities;
using WebApi.Exceptions;
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
    private readonly DataContext _context;
    private readonly PopulatorService _populatorService;

    public UsersController(
        UserService userService,
        IOptions<AppSettings> appSettings,
        DataContext dataContext,
        PopulatorService populatorService)
    {
        _userService = userService;
        _appSettings = appSettings.Value;
        _context = dataContext;
        _populatorService = populatorService;
    }

    /// <summary>
    /// Login
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpPost("authenticate")]
    public IActionResult Authenticate(AuthenticateRequest model)
    {
        var response = _userService.Authenticate(model);
        return Ok(response);
    }

    /// <summary>
    /// Register user
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpPost("register")]
    public IActionResult Register(RegisterRequest model)
    {
        _userService.Register(model);
        return Ok(new { message = "Registration successful" });
    }

    /// <summary>
    /// Get basic info
    /// </summary>
    /// <returns></returns>
    [HttpGet("basicinfo")]
    public BasicInfoResponse GetBasicInfo()
    {
        var user = (User)HttpContext.Items["User"];

        return _populatorService.PopulateBasicInfoResponse(user);
    }

    /// <summary>
    /// Trigger a password reset
    /// </summary>
    /// <param name="email">User username or email</param>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpPost]
    [Route("trigger-reset-password")]
    public async Task TriggerResetPassword([FromBody] string email)
    {
        await _userService.TriggerResetPasswordAsync(email);
    }

    /// <summary>
    /// Reset password
    /// </summary>
    /// <param name="resetpasscode">Unique code to reset</param>
    /// <param name="newpassword">New password</param>
    [AllowAnonymous]
    [HttpPost]
    [Route("reset-password")]
    public void ResetPassword([FromQuery] string resetpasscode, [FromBody] string newpassword)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(resetpasscode)) throw new InvalidOperationException();

            var user = _context.Users.First(t => t.ResetPasswordCode == resetpasscode);

            // reset resetpass so you can't reset your password again using the same URL
            user.ResetPasswordCode = null;

            _userService.ResetPassword(user.Id, newpassword);
        }
        catch (InvalidOperationException)
        {
            throw new HandledException(HttpStatusCode.BadRequest, "Reset code is not valid.");
        }
    }
}