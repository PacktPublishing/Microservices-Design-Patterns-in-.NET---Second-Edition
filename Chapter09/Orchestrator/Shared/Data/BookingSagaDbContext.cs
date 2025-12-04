using Microsoft.EntityFrameworkCore;

namespace Shared.Data;

public class BookingSagaDbContext : DbContext
{
    public BookingSagaDbContext(DbContextOptions<BookingSagaDbContext> options) : base(options) { }

    public DbSet<BookingSagaState> BookingStates { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BookingSagaState>(cfg =>
        {
            cfg.HasKey(x => x.CorrelationId);
        });

        base.OnModelCreating(modelBuilder);
    }
}