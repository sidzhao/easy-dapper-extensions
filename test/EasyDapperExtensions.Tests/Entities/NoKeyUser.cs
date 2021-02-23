using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasyDapperExtensions.Tests.Entities
{
    [Table("NoKeyUsers")]
    public class NoKeyUser
    {
        public Guid Key { get; set; }

        public string Name { get; set; }
    }
}
