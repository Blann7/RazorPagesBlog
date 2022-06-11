namespace BlogPhone.Models
{
    public class User
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Password { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
        public long? RoleValidityMs { get; set; } // role valide to this milliseconds (next - reset to user)
        public long? BanMs { get; set; } // banned to this milliseconds
        public int Money { get; set; } = 0;

        public User() { }

        public User(string name, string password, string email, string role)
        {
            Name = name;
            Password = password;
            Email = email;
            Role = role;
        }
    }
}
