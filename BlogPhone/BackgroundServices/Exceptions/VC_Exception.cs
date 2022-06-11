using BlogPhone.BackgroundServices.Messages;

namespace BlogPhone.BackgroundServices.Exceptions
{
    public class VC_Exception : Exception
    {
        public VC_Exception(string who, string message) : base(message)
        {
            Error.Print(who, message);
        }
    }
}
