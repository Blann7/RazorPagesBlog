namespace BlogPhone.Models
{
    public class AboutUsInfo
    {
        public int Id { get; set; }
        // paragraph 1
        public string? P1_Title { get; set; } 
        public string? P1_Text { get; set; }
        public byte[]? P1_ImageData { get; set; }
        // paragraph 2
        public string? P2_Title { get; set; }
        public string? P2_Text { get; set; }
        public byte[]? P2_ImageData { get; set; }
        public AboutUsInfo() { }
    }
}
