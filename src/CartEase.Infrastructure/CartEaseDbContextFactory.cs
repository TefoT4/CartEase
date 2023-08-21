using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace CartEase.Infrastructure
{
    internal class CartEaseDbContextFactory : IDesignTimeDbContextFactory<CartEaseContext>
    {
        CartEaseContext IDesignTimeDbContextFactory<CartEaseContext>.CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var builder = new DbContextOptionsBuilder<CartEaseContext>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            builder.UseSqlServer(connectionString);

            return new CartEaseContext(builder.Options);
        }
    }
}
