using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wolverine.EntityFrameworkCore;
using Yestino.Common.Infrastructure.Persistence;
using Yestino.Order.Infrastructure;

namespace Yestino.Order;

public static class DependencyInjection
{
    public static WebApplicationBuilder AddOrderModule(this WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("YestinoDb");

        builder.Services.AddDbContextWithWolverineIntegration<OrderDbContext>(options =>
            options.UseNpgsql(connectionString));

        builder.Services
            .AddScoped<IAggregateRepository<Entities.Order>, EfAggregateRepository<Entities.Order, OrderDbContext>>()
            ;

        return builder;
    }
}
