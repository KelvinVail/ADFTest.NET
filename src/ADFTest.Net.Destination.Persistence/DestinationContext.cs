using ADFTest.Net.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ADFTest.Net.Destination.Persistence
{
    public class DestinationContext : DbContext
    {
        public DestinationContext(DbContextOptions<DestinationContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Author> Authors { get; set; }
    }
}
