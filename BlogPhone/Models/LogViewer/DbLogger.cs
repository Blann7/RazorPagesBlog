using static LogViewer.Models.LogTypes;

namespace BlogPhone.Models.LogViewer
{
    public class DbLogger
    {
        public DbLogger() { }
        public void Add(int userId, string userName, LogType logType, string message)
        {
            LogActions.SaveLog(userId, userName, logType, message);
        }
    }
}
