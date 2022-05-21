namespace BlogPhone.Models
{
    public static class ImageWorking
    {
        public static byte[] GetImageDataFromIFormFile(IFormFile? image)
        {
            if(image is null) throw new Exception("DB FAILURE | method - ImageWorking.GetImageDataFromIFormFile(IFormFile? image)");

            byte[]? imageData;
            using (BinaryReader br = new BinaryReader(image.OpenReadStream()))
            {
                imageData = br.ReadBytes((int)image.Length);
            }

            return imageData;
        }
        public static string GetImageURLFromBytesArray(byte[]? imageData)
        {
            if (imageData is null) throw new Exception("DB FAILURE | method - ImageWorking.GetImageURLFromBytesArray(byte[]? imageData)");

            string imreBase64Data = Convert.ToBase64String(imageData);
            string imgDataURL = string.Format("data:image/png;base64,{0}", imreBase64Data);

            return imgDataURL;
        }
        public static async Task<byte[]> GetImageDataFromDefaultImageAsync()
        {
            byte[]? imageData;
            using (FileStream fs = new FileStream($"{Environment.CurrentDirectory}/wwwroot/images/default-thumbnail.jpg", FileMode.Open))
            {
                byte[] fsbyte = new byte[fs.Length];
                await fs.ReadAsync(fsbyte);

                imageData = fsbyte;
            }
            return imageData;
        }
    }
}
