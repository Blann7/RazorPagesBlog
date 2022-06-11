namespace BlogPhone.BackgroundServices.Messages
{
    public static class Info
    {
        public static void Print(string message)
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write("\t" + message);

            Console.WriteLine();
            Console.ResetColor();
        }
        public static void Print(string who, string message)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write($"\t [{who}] ");

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write(message);

            Console.WriteLine();
            Console.ResetColor();
        }
    }
}
