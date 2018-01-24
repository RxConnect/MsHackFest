using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RxConnectSite.Models;

namespace RxConnectSite.Data
{
    public class RxConnectSiteContext : DbContext
    {
        public RxConnectSiteContext (DbContextOptions<RxConnectSiteContext> options)
            : base(options)
        {
        }

        public DbSet<RxConnectSite.Models.Fobs> Fobs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Fobs>().ToTable("Fobs");
        }
    }
}
