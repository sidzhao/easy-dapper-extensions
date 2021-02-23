using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyDapperExtensions
{
    public class PagedResult<TEntity>
    {
        public IEnumerable<TEntity> Result { get; set; }

        public int TotalCount { get; set; }
    }
}
