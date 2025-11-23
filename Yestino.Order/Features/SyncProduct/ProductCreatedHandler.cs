using Yestino.Order.Entities;
using Yestino.Order.Infrastructure;
using Yestino.ProductCatalogContracts.DomainEvents;

namespace Yestino.Order.Features.SyncProduct;

public class ProductCreatedHandler(OrderDbContext db)
{
    public async Task Handle(ProductCreated evt)
    {
        var exists = await db.ProductReadModels.FindAsync(evt.AggregateId);

        if (exists != null)
            return;

        var product = new ProductReadModel
        {
            Id = evt.AggregateId,
            Name = evt.Name,
            Description = evt.Description,
            ImageUrl = evt.ImageUrl,
            Price = evt.Price
        };

        db.ProductReadModels.Add(product);

        await db.SaveChangesAsync();
    }
}
