namespace BlogPhone.Models
{
    public static class KeyGen
    {
        private readonly static Random random = new Random();
        private static char[] chars = new[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P',
            'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm',
            'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0',
            '_', '-', '+', '!', '@', '#', '$', '%', '^', '*'}; // Length = 72

        public static string GetKey(int length)
        {
            string key = "";

            for (int i = 0; i < length; i++)
            {
                key += chars[random.Next(71)];
            }

            return key;
        }
    }
}
