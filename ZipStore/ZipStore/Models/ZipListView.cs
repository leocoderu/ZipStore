using System.Collections.Generic;
using ZipStore.Entities;

namespace ZipStore.Models
{
    public class ZipListViewModel
    {
        public IEnumerable<ZipItem> ZipItems { get; set; }

    }
}