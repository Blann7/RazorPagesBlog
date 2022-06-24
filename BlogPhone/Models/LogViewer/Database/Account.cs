namespace BlogPhone.Models.LogViewer.Database
{
    public class Account
    {
        public int Id { get; set; }
        public string Login { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? RefreshToken { get; set; }
        public long TokenExpirate { get; set; }
        public string? LastIp { get; set; }
    }
}
