using Yestino.OrderContracts.DomainEvents;
using Yestino.Warehouse.Infrastructure;

namespace Yestino.Warehouse.Application;

public class OrderPayedHandler(WarehouseDbContext db)
{
    private readonly WarehouseDbContext _db = db;

    public async Task Handle(OrderPayed @event)
    {
        // TODO: rezervuj polozku na sklade
    }
}
