using Newtonsoft.Json;

namespace WinArto
{
    /// <summary>
    ///     Class TestSet.
    /// </summary>
    public class TestSet
    {
        /// <summary>
        ///     Gets or sets the local path.
        /// </summary>
        /// <value>The local path.</value>
        [JsonProperty("local")]
        public string LocalPath { get; set; }

        /// <summary>
        ///     Gets or sets the remote path.
        /// </summary>
        /// <value>The remote path.</value>
        [JsonProperty("remote")]
        public string RemotePath { get; set; }
    }
}