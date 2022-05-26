using Newtonsoft.Json;

namespace ValidateChecker.Configuration
{
    internal static class Init
    {
        internal static int GetInterval()
        {
            ConfigFileModel? jsonData = JsonConvert.DeserializeObject<ConfigFileModel>(File.ReadAllText($"{Environment.CurrentDirectory}\\config.json"));

            return jsonData!.Interval;
        }
    }
}
