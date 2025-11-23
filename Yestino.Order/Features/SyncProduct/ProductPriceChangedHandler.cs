using Yestino.Order.Infrastructure;
using Yestino.ProductCatalogContracts.DomainEvents;

namespace Yestino.Order.Features.SyncProduct;

public class ProductPriceChangedHandler(OrderDbContext db)
{
    public async Task Handle(ProductPriceChanged evt)
    {
        var product = await db.ProductReadModels.FindAsync(evt.AggregateId);

        if (product == null)
            return;

        product.Price = evt.NewPrice;

        await db.SaveChangesAsync();
    }
}