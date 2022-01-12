namespace WebApi.Services;

using BCryptNet = BCrypt.Net.BCrypt;
using WebApi.Authorization;
using WebApi.Entities;
using WebApi.Helpers;
using WebApi.Models.Users;
using System.Net;

public class UserService
{
    private DataContext _context;
    private IJwtUtils _jwtUtils;

    public UserService(
        DataContext context,
        IJwtUtils jwtUtils)
    {
        _context = context;
        _jwtUtils = jwtUtils;
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

        if(!string.IsNullOrEmpty(model.Sponsor))
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
}