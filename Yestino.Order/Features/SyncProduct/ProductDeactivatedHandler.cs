using Yestino.Order.Infrastructure;
using Yestino.ProductCatalogContracts.DomainEvents;

namespace Yestino.Order.Features.SyncProduct;

public class ProductDeactivatedHandler(OrderDbContext db)
{
    public async Task Handle(ProductDeactivated evt)
    {
        var product = await db.ProductReadModels.FindAsync(evt.AggregateId);

        if (product == null)
            return;

        product.IsActive = false;

        await db.SaveChangesAsync();
    }
}
