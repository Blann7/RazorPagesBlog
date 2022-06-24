namespace BlogPhone.Models.LogViewer.Database
{
    public class LogString
    {
        public int Id { get; set; }
        public string Type { get; set; } = null!;
        public int UserId { get; set; }
        public string Message { get; set; } = null!;
        public long CreatedAt { get; set; }
        public string UserName { get; set; } = null!;
        public string CreatedAtDateString { get; set; } = null!;
    }
}
