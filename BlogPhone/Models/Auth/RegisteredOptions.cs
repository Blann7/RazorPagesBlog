namespace BlogPhone.Models.Auth
{
    public class RegisteredOptions
    {
        public Guid regKey = new Guid();
        
        readonly static Random random = new Random();
        public static List<string> regKeys { get; set; } = new();

        public RegisteredOptions()
        {
            regKeys.Add(regKey.ToString());
        }
    }
}