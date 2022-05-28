namespace ValidateChecker
{
    class Program
    {
        static void Main(string[] args)
        {
            int interval = Configuration.Init.GetInterval();

            while (true)
            {
                Console.WriteLine("Старт итерации проверки");

                RoleValidityChecker rvc = new RoleValidityChecker();
                rvc.StartCheckAsync();

                Console.WriteLine("_______выполнено_______");

                Thread.Sleep(interval); // change
            }
        }
    }
}
