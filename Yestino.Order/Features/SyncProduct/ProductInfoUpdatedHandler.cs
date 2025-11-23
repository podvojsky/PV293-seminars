using Yestino.Order.Infrastructure;
using Yestino.ProductCatalogContracts.DomainEvents;

namespace Yestino.Order.Features.SyncProduct;

public class ProductInfoUpdatedHandler(OrderDbContext db)
{
    public async Task Handle(ProductInfoUpdated evt)
    {
        var product = await db.ProductReadModels.FindAsync(evt.AggregateId);

        if (product == null)
            return;

        product.Name = evt.Name;
        product.Description = evt.Description;
        product.ImageUrl = evt.ImageUrl;

        await db.SaveChangesAsync();
    }
}
