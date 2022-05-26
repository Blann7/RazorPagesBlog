using Newtonsoft.Json;

namespace ValidateChecker.Configuration
{
    internal class ConfigFileModel
    {
        [JsonProperty("exclude")] public string[] Exclude { get; set; } = Array.Empty<string>();
        [JsonProperty("interval")] public int Interval { get; set; }
    }
}
