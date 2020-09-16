using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Keeper.Server.Database.Models
{
    [Table("accounts")]
    public class AccountDto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public uint id { get; set; }
        [ForeignKey(nameof(user))]
        public uint user_id { get; set; }
        public UserDto user { get; set; }
        public string account { get; set; }
        public string account_id { get; set; }
        public string account_password { get; set; }
    }
}
