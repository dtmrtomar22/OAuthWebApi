using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;

namespace ProductWebAPI.Models
{
    public class ProductAuthDbContext : IdentityDbContext
    {
        public ProductAuthDbContext() : base("OwinAuthDbContext") { }         
    }
}