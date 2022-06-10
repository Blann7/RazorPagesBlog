namespace BlogPhone.Models
{
    public class ArticleBlog
    {
        public int Id { get; set; }
        public string? Label { get; set; }
        public string? ShortArticleText { get; set; }
        public string? ArticleText { get; set; }
        public byte[]? ImageData { get; set; }
        public string? PublishDate { get; set; }

        public ArticleBlog()
        { }

        public ArticleBlog(string label, string articleText, byte[]? imageData)
        {
            Label = label;
            ArticleText = articleText;
            ImageData = imageData;
            PublishDate = DateTime.Now.ToShortDateString();
            SetShortArticleText();
        }
        public void SetShortArticleText()
        {
            string shortArticleText = "";
            if (ArticleText!.Length > 50)
            {
                for (int i = 0; i < 50; i++)
                {
                    shortArticleText += ArticleText[i];
                }

                ShortArticleText = shortArticleText + "...";
            }
            else ShortArticleText = ArticleText;
        }
    }
}
