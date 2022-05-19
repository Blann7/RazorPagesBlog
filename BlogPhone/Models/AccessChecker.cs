namespace BlogPhone.Models
{
    public static class AccessChecker
    {
        public static bool Check(in string? userRole, params string[] allowedRoles)
        {
            if (userRole is null) return false;

            foreach (string role in allowedRoles)
            {
                if(role == userRole) 
                    return true;
            }

            return false;
        }
    }
}
