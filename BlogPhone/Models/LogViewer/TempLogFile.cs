using BlogPhone.Models.LogViewer.Database;
using Newtonsoft.Json;
using System.Text;

namespace BlogPhone.Models.LogViewer
{
    public static class TempLogFile
    {
        public static void Create()
        {
            File.Create($"{Environment.CurrentDirectory}\\LogViewer_TempLog.txt"); // создание
        }
        public static void Clear()
        {
            using (StreamWriter sw = new StreamWriter($"{Environment.CurrentDirectory}\\LogViewer_TempLog.txt", false, Encoding.UTF8))
            {
                sw.WriteLine(string.Empty);
            }
        }
    }
}
