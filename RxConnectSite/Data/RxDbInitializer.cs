using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RxConnectSite.Models;

namespace RxConnectSite.Data
{
    public class RxDbInitializer
    {
        public static void Initialize(RxConnectSiteContext dbContext)
        {
            dbContext.Database.EnsureCreated();

            if (dbContext.Fobs.Any())
            {
                //db already initialized
                return;
            }
        }
    }
}
