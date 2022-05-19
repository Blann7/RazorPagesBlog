namespace BlogPhone.Models
{
    /// <summary>
    /// methods returns 'true' if user approved; false - if access prohibited
    /// </summary>
    public static class AccessChecker
    {
        public const string EMAIL = "BobAdmin@mail.ru";
        public static bool RoleCheck(in string? userRole, params string[] allowedRoles)
        {
            if (userRole is null) return false;

            foreach (string role in allowedRoles)
            {
                if(role == userRole) 
                    return true;
            }

            return false;
        }
        public static bool BanCheck(in string? userBanDate)
        {
            if(userBanDate is null) return true;

            DateTime banDate = DateTime.Parse(userBanDate);
            if(DateTime.UtcNow < banDate) return false;
            
            return true;
        }
    }
}
