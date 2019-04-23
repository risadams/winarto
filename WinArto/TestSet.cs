using Newtonsoft.Json;

namespace WinArto
{
    /// <summary>
    ///     Class TestSet.
    /// </summary>
    public class TestSet
    {
        [JsonProperty("breaks")]
        public BreakPoint[] BreakPoints { get; set; }

        [JsonProperty("local")]
        public string LocalDomain { get; set; }

        [JsonProperty("paths")]
        public string[] Paths { get; set; }

        [JsonProperty("remote")]
        public string RemoteDomain { get; set; }
    }

    public class BreakPoint
    {
        [JsonProperty("width")]
        public int Width { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}