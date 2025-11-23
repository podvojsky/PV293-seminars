using Yestino.Order.Infrastructure;
using Yestino.ProductCatalogContracts.DomainEvents;

namespace Yestino.Order.Features.SyncProduct;

public class ProductActivatedHandler(OrderDbContext db)
{
    public async Task Handle(ProductActivated evt)
    {
        var product = await db.ProductReadModels.FindAsync(evt.AggregateId);

        if (product == null)
            return;

        product.IsActive = true;

        await db.SaveChangesAsync();
    }
}
