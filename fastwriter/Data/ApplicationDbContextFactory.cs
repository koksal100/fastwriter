using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace fastwriter.Data;

/// <summary>
/// dotnet ef komutlarının uygulamayı çalıştırmadan DbContext oluşturmasını sağlar.
/// </summary>
public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite("Data Source=fastwriter.db")
            .Options;

        return new ApplicationDbContext(options);
    }
}
