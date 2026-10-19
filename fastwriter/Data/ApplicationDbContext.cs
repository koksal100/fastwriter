using fastwriter.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace fastwriter.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<TypingText> TypingTexts => Set<TypingText>();

    public DbSet<Score> Scores => Set<Score>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Score>(entity =>
        {
            entity.HasOne(s => s.User)
                .WithMany(u => u.Scores)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(s => s.TypingText)
                .WithMany(t => t.Scores)
                .HasForeignKey(s => s.TypingTextId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
