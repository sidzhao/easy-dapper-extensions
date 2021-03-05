using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyDapperExtensions.Tests.Entities
{
    [Table("InsertUserUseIds")]
    public class InsertUserUseId
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
