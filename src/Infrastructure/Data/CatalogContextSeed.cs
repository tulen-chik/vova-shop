using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.eShopWeb.ApplicationCore.Entities;
using Microsoft.Extensions.Logging;

namespace Microsoft.eShopWeb.Infrastructure.Data;

public class CatalogContextSeed
{
    public static async Task SeedAsync(CatalogContext catalogContext,
        ILogger logger,
        int retry = 0)
    {
        var retryForAvailability = retry;
        try
        {
            if (catalogContext.Database.IsSqlServer())
            {
                catalogContext.Database.Migrate();
            }

            if (!await catalogContext.CatalogBrands.AnyAsync())
            {
                await catalogContext.CatalogBrands.AddRangeAsync(
                    GetPreconfiguredCatalogBrands());

                await catalogContext.SaveChangesAsync();
            }

            if (!await catalogContext.CatalogTypes.AnyAsync())
            {
                await catalogContext.CatalogTypes.AddRangeAsync(
                    GetPreconfiguredCatalogTypes());

                await catalogContext.SaveChangesAsync();
            }

            if (!await catalogContext.CatalogItems.AnyAsync())
            {
                await catalogContext.CatalogItems.AddRangeAsync(
                    GetPreconfiguredItems());

                await catalogContext.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            if (retryForAvailability >= 10) throw;

            retryForAvailability++;
            
            logger.LogError(ex.Message);
            await SeedAsync(catalogContext, logger, retryForAvailability);
            throw;
        }
    }

    static IEnumerable<CatalogBrand> GetPreconfiguredCatalogBrands()
    {
        return new List<CatalogBrand>
            {
                new("Nike"),
                new("Adidas"),
                new("Puma"),
                new("Reebok"),
                new("New Balance")
            };
    }

    static IEnumerable<CatalogType> GetPreconfiguredCatalogTypes()
    {
        return new List<CatalogType>
            {
                new("Беговые кроссовки"),
                new("Баскетбольные кроссовки"),
                new("Кэжуал кроссовки"),
                new("Тренировочные кроссовки")
            };
    }

    static IEnumerable<CatalogItem> GetPreconfiguredItems()
    {
        return new List<CatalogItem>
            {
                new(2,2, "Nike Air Max 270", "Nike Air Max 270", 150.5M,  "http://catalogbaseurltobereplaced/images/products/1.jpg"),
                new(1,2, "Adidas Ultraboost", "Adidas Ultraboost", 180.50M, "http://catalogbaseurltobereplaced/images/products/2.jpg"),
                new(2,5, "Puma RS-X", "Puma RS-X", 120,  "http://catalogbaseurltobereplaced/images/products/3.jpg"),
                new(2,2, "Reebok Classic Leather", "Reebok Classic Leather", 12, "http://catalogbaseurltobereplaced/images/products/4.jpg"),
                new(3,5, "New Balance 990v5", "New Balance 990v5", 8.5M, "http://catalogbaseurltobereplaced/images/products/5.jpg"),
            };
    }
}
