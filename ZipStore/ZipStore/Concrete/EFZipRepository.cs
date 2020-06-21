using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ZipStore.Entities;
using ZipStore.Abstract;

namespace ZipStore.Concrete
{
    
    public class EFZipRepository : IZipRepository
    {
        private EFDBContext context = new EFDBContext();

        public IEnumerable<ZipItem> ZipItems 
        {
            get { return context.ZipItems; }
        }
    }
}