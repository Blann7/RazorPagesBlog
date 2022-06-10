using System.Text;

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
    }
}
