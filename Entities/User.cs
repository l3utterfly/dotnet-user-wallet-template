namespace WebApi.Entities;

public class User
{
    public User()
    {
        Referrals = new HashSet<User>();
    }

    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Username { get; set; }
    public string PasswordHash { get; set; }
    public string Email { get; set; }
    public DateTime Created { get; set; }
    public int? SponsorId { get; set; }

    public virtual User Sponsor { get; set; }
    public virtual ICollection<User> Referrals { get; set; }
}