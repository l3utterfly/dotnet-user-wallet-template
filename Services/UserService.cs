namespace WebApi.Services;

using BCryptNet = BCrypt.Net.BCrypt;
using WebApi.Authorization;
using WebApi.Entities;
using WebApi.Helpers;
using WebApi.Models.Users;
using System.Net;
using WebApi.Exceptions;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

public class UserService
{
    private DataContext _context;
    private IJwtUtils _jwtUtils;
    private readonly EmailService _emailService;

    private readonly char[] chars =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();

    public UserService(
        DataContext context,
        IJwtUtils jwtUtils,
        EmailService emailService)
    {
        _context = context;
        _jwtUtils = jwtUtils;
        _emailService = emailService;
    }

    public AuthenticateResponse Authenticate(AuthenticateRequest model)
    {
        var user = _context.Users.SingleOrDefault(x => x.Username == model.Username);

        // validate
        if (user == null || !BCryptNet.Verify(model.Password, user.PasswordHash))
            throw new HandledException(HttpStatusCode.BadRequest, "Username or password is incorrect");

        // authentication successful
        var response = new AuthenticateResponse()
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
        };

        response.Token = _jwtUtils.GenerateToken(user);
        return response;
    }

    public void Register(RegisterRequest model)
    {
        // validate
        if (_context.Users.Any(x => x.Username == model.Username || x.Email == model.Email))
            throw new HandledException(HttpStatusCode.Conflict, $"Username ${model.Username} or email {model.Email} is already taken");

        int? sponsorId = null;

        if (!string.IsNullOrEmpty(model.Sponsor))
        {
            var sponsor = _context.Users.FirstOrDefault(t => t.Username == model.Sponsor);
            if (sponsor == null)
                throw new HandledException(HttpStatusCode.BadRequest, $"Sponsor user {model.Sponsor} does not exist.");

            sponsorId = sponsor.Id;
        }

        // map model to new user object
        var user = new User()
        {
            Username = model.Username,
            FirstName = model.FirstName,
            LastName = model.LastName,
            Email = model.Email,
            Created = DateTime.UtcNow,
            SponsorId = sponsorId
        };

        // hash password
        user.PasswordHash = BCryptNet.HashPassword(model.Password);

        // save user
        _context.Users.Add(user);
        _context.SaveChanges();
    }

    public async Task TriggerResetPasswordAsync(string email)
    {
        var user = _context.Users.FirstOrDefault(t => t.Email == email);

        if (user == null)
            throw new HandledException(HttpStatusCode.BadRequest, $"No user with email {email} found.");

        // update reset pass
        var resetpass = GetRandomString(6);

        user.ResetPasswordCode = resetpass;

        await _context.SaveChangesAsync();

        // trigger reset email
        await _emailService.SendEmail(user.Email, "Please reset your password", $"Your reset code: {resetpass}");

    }

    private string GetRandomString(int length)
    {
        byte[] data = new byte[4 * length];
        using (var crypto = RandomNumberGenerator.Create())
        {
            crypto.GetBytes(data);
        }

        StringBuilder result = new StringBuilder(length);
        for (int i = 0; i < length; i++)
        {
            var rnd = BitConverter.ToUInt32(data, i * 4);
            var idx = rnd % chars.Length;

            result.Append(chars[idx]);
        }

        return result.ToString();
    }

    /// <summary>
    /// Reset password for user. This will set the user password to <paramref name="newpassword"/>, and regenerate a salt for the user
    /// </summary>
    /// <param name="userid"></param>
    /// <param name="newpassword"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"><paramref name="newpassword"/> is null or whitespace</exception>
    public User ResetPassword(int userid, string newpassword)
    {
        if (string.IsNullOrWhiteSpace(newpassword)) throw new ArgumentNullException(nameof(newpassword), "Input password cannot be empty.");

        var user = _context.Users.First(t => t.Id == userid);

        user.PasswordHash = BCryptNet.HashPassword(newpassword);

        _context.SaveChanges();

        return user;
    }
}