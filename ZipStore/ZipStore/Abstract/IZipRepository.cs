using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZipStore.Entities;

namespace ZipStore.Abstract
{
    public interface IZipRepository
    {
        IEnumerable<ZipItem> ZipItems { get; }
    }
}
