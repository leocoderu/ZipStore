using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using ZipStore.Entities;

namespace ZipStore.Concrete
{
    public class EFDBContext : DbContext
    {
        public DbSet<ZipItem> ZipItems { get; set; }
    }
}