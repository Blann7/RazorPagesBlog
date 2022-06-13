using Microsoft.EntityFrameworkCore;

namespace BlogPhone.Models
{
    public class Referral
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string? Code { get; set; }
        public int InvitedUsers { get; set; } = 0;

        public const string DOMAIN = "u1690754.plsk.regruhosting.ru/"; // for link view (office/user)
        public const int USER_MONEY_BONUS = 500; // bonus money for man, WHO invite user

        public const int NEWUSER_MONEY_BONUS = 250; // bonus money for invited man
        public const int NEWUSER_DAYROLE_BONUS = 7; // bonus day of role for invited man
        public const string NEWUSER_ROLE_BONUS = "standart"; // bonus money for invited man
        
        private static char[] data = new char[] 
        { 
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
            '1', '2', '3', '4', '5', '6', '7', '8', '9', '0'
        };
        
        public Referral() { }
        public Referral(int userId)
        {
            Code = GetCode().Result;
            if (string.IsNullOrEmpty(Code)) 
                throw new Exception("Referral.Code is null of empty (not expected value)");

            UserId = userId;
        }
        private async Task<string> GetCode()
        {
            bool repeat = false;
            do
            {
                Code = GenerateCode();
                repeat = await IsCodeAlreadyExist(Code);
                
                if(repeat == false) return Code;
            } 
            while (repeat == true);
            
            return ""; // gag, not expected value
        }
        private string GenerateCode()
        {
            Random r = new Random();

            Code = "";
            for (int i = 0; i < 6; i++)
                Code += data[r.Next(0, data.Length)]; // Code generated
            return Code;
        }
        /// <returns>True if exist, false - not exist</returns>
        public static async Task<bool> IsCodeAlreadyExist(string code)
        {
            using (ApplicationContext context = new ApplicationContext())
            {
                foreach (Referral referral in await context.Referrals.ToListAsync())
                {
                    if (referral.Code == code) return true; // in case of repeat
                }
            }
            return false;
        }
    }
}
