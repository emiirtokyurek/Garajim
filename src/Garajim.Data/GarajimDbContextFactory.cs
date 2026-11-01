using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Garajim.Data;

/// <summary>Sadece "dotnet ef migrations" komutları için kullanılır; uygulama çalışırken devreye girmez.</summary>
public sealed class GarajimDbContextFactory : IDesignTimeDbContextFactory<GarajimDbContext>
{
    public GarajimDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<GarajimDbContext>()
            .UseSqlite("Data Source=design-time.db")
            .Options;
        return new GarajimDbContext(options);
    }
}
