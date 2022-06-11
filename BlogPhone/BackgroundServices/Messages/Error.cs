namespace BlogPhone.BackgroundServices.Messages
{
    public static class Error
    {
        public static void Print(string err_message)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.Write("\t" + err_message);

            Console.WriteLine();
            Console.ResetColor();
        }
        public static void Print(string who, string err_message)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write($"\t[{who}] ");

            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.Write(err_message);

            Console.WriteLine();
            Console.ResetColor();
        }
    }
}
