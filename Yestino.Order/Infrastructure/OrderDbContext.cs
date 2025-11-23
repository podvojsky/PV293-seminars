using Microsoft.EntityFrameworkCore;
using Wolverine;
using Yestino.Common.Infrastructure.Persistence;
using Yestino.Order.Entities;

namespace Yestino.Order.Infrastructure;

public class OrderDbContext(DbContextOptions<OrderDbContext> options, IMessageBus bus)
    : DbContextBase(options, bus)
{
    public DbSet<Entities.Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<ProductReadModel> ProductReadModels { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("order");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrderDbContext).Assembly);
    }
}
