using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TradingSite.Data;

namespace TradingSite.Database
{
    public class AppDatabaseContext : DbContext
    {
        public AppDatabaseContext(DbContextOptions<AppDatabaseContext> options)
        :base(options)
        {

        }

        public DbSet<Item> Items { get; set; }
    }
}
