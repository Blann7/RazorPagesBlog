namespace BlogPhone.Models
{
    public class ArticleBlog
    {
        public int Id { get; set; }
        public string? Label { get; set; }
        public string? ShortArticleText { get; set; }
        public string? AricleText { get; set; }
        public byte[]? ImageData { get; set; }
        public string? PublishDate { get; set; }

        public ArticleBlog()
        { }

        public ArticleBlog(string label, string shortArticleText, string articleText, byte[] imageData, string publishDate)
        {
            Label = label;
            ShortArticleText = shortArticleText;
            AricleText = articleText;
            ImageData = imageData;
            PublishDate = publishDate;
        }
    }
}
