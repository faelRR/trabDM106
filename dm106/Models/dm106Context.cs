using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace dm106.Models
{
    public class dm106Context : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx
    
        public dm106Context() : base("name=dm106Context")
        {
        }

        public System.Data.Entity.DbSet<dm106.Models.Product> Products { get; set; }

        public System.Data.Entity.DbSet<dm106.Models.Order> Orders { get; set; }

        public System.Data.Entity.DbSet<dm106.Models.OrderItem> OrderItems { get; set; }
    }
}
