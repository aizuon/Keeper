using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Keeper.Server.Database.Models
{
    [Table("users")]
    public class UserDto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public uint id { get; set; }
        public string username { get; set; }
        public string password { get; set; }

        public IList<AccountDto> Accounts { get; set; } = new List<AccountDto>();
    }
}
