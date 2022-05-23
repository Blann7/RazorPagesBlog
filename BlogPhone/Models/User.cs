namespace BlogPhone.Models
{
    public class User
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Password { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
        public string? RoleValidityDate { get; set; } // role valide to this date (next - reset to user)
        public string? BanDate { get; set; } // banned to this date
        public int Money { get; set; } = 0;

        public User()
        { }

        public User(string name, string password, string email, string role)
        {
            Name = name;
            Password = password;
            Email = email;
            Role = role;
        }
    }
}
