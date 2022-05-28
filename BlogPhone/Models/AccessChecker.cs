namespace BlogPhone.Models
{
    /// <summary>
    /// methods returns 'true' if user approved; false - if access prohibited
    /// </summary>
    public static class AccessChecker
    {
        public const string EMAIL = "BobAdmin@mail.ru";
        public const int USER_COST = 0;
        public const int STANDART_COST = 600;
        public const int MODER_COST = 1100;
        public const int ADMIN_COST = 2000;
        /// <summary>
        /// Returns true if user have at least 1 allowedRole
        /// </summary>
        /// <param name="userRole"></param>
        /// <param name="allowedRoles"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Returns FALSE when user banned, Time sync with UTC
        /// </summary>
        /// <param name="userBanDate"></param>
        /// <returns></returns>
        public static bool BanCheck(in string? userBanDate)
        {
            if(userBanDate is null) return true;

            DateTime banDate = DateTime.Parse(userBanDate);
            if(DateTime.UtcNow < banDate) return false;
            
            return true;
        }
    }
}
