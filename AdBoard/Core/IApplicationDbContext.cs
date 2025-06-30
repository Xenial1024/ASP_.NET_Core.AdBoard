using AdBoard.Core.Models.Domains;
using Microsoft.EntityFrameworkCore;

namespace AdBoard.Core
{
    public interface IApplicationDbContext
    {
        DbSet<Ad> Ads { get; set; }
        DbSet<Image> Images { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}