using System.Data.Entity;
using ZipStore.Entities;

namespace ZipStore.Concrete
{
    public class EFDBContext : DbContext
    {
        public DbSet<ZipItem> ZipItems { get; set; }
    }
}