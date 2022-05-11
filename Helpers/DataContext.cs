namespace WebApi.Helpers;

using Microsoft.EntityFrameworkCore;
using WebApi.Entities;

public class DataContext : DbContext
{
    protected readonly IConfiguration Configuration;

    public DbSet<User> Users { get; set; }

    public DbSet<InternalTransaction> InternalTransacions { get; set; }

    public DataContext(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        // connect to sql server database
        options.UseSqlServer(Configuration.GetConnectionString("WebApiDatabase"));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.FirstName).IsRequired();
            entity.Property(e => e.Username).IsRequired();
            entity.Property(e => e.LastName).IsRequired();
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.Email).IsRequired();

            entity.HasIndex(e => e.SponsorId).IsUnique();

            entity.HasOne(e => e.Sponsor)
                .WithMany(e => e.Referrals)
                .HasForeignKey(e => e.SponsorId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasOne(e => e.User)
                .WithMany(e => e.AllTransactions)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.Property(e => e.Amount).HasPrecision(38, 8);
        });

        modelBuilder.Entity<InternalTransaction>(entity =>
        {
            entity.HasBaseType<Transaction>();

            entity.HasOne(e => e.FromUser)
                .WithMany(e => e.InternalTransactions)
                .HasForeignKey(e => e.FromUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ManualTransaction>(entity =>
        {
            entity.HasBaseType<Transaction>();
        });
    }
}