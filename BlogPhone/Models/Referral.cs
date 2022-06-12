using Microsoft.EntityFrameworkCore;

namespace BlogPhone.Models
{
    public class Referral
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string? Code { get; set; }

        public const string DOMAIN = "u1690754.plsk.regruhosting.ru/"; // for link view (office/user)
        
        private static char[] data = new char[] 
        { 
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', 
            '1', '2', '3', '4', '5', '6', '7', '8', '9', '0' 
        };
        
        public Referral() { }
        public Referral(int userId)
        {
            Random r = new Random();

            Code = "";
            for (int i = 0; i < 6; i++) 
                Code += data[r.Next(0, data.Length)];

            UserId = userId;
        }
    }
}
