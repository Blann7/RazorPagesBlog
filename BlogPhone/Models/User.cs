using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogPhone.Models
{
    public class User
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Password { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
        public bool FullDostup { get; set; } = false;
        public long? RoleValidityMs { get; set; } = 0; // role valide to this milliseconds (next - reset to user)
        public long? BanMs { get; set; } = 0; // banned to this milliseconds
        [Required, Column(TypeName = "decimal(18,2)")] public decimal Money { get; set; } = 0;
        
        public User() { }
    }
}
