using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace McBiboo
{
    public class ManifestHeader
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("uuid")]
        public string? Uuid { get; set; }

        [JsonPropertyName("version")]
        public int[]? Version { get; set; }
    }

    public class ManifestModule
    {
        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("uuid")]
        public string? Uuid { get; set; }

        [JsonPropertyName("version")]
        public int[]? Version { get; set; }
    }

    public class ManifestRef
    {
        [JsonPropertyName("format_version")]
        public int? FormatVersion { get; set; }

        [JsonPropertyName("header")]
        public ManifestHeader? Header { get; set; }

        [JsonPropertyName("modules")]
        public List<ManifestModule>? Modules { get; set; }
    }
}
