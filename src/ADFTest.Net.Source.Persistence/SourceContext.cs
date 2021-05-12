using ADFTest.Net.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ADFTest.Net.Source.Persistence
{
    public class SourceContext : DbContext
    {
        public SourceContext(DbContextOptions<SourceContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Author> Authors { get; set; }
    }
}
