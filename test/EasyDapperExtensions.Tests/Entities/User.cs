using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasyDapperExtensions.Tests.Entities
{
    public enum UserStatus
    {
        Active,
        Inactive
    }

    [Table("Users")]
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Name { get; set; }

        public UserStatus Status { get; set; }

        public DateTime CreatedTime { get; set; }

        public bool? IsStatic { get; set; }
    }
}
