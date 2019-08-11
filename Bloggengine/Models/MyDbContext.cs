using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Bloggengine.Controllers;

namespace Bloggengine.Models
{
    public class MyDbContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }
        
        public DbSet<Blogs> Blogs { get; set; }

        public DbSet<Likes> LikeConns { get; set; }


    }
}