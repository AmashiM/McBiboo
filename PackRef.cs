using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace McBiboo
{

    public class PackRef
    {
        [JsonPropertyName("pack_id")]
        public required string PackId { get; set; }

        [JsonPropertyName("version")]
        public required int[] Version { get; set; }
    }

    public class PackRefBase
    {
        public static int FromManifestString(string manifestContent, out PackRef? packRef)
        {
            packRef = null;

            var node = JsonObject.Parse(Server.JsonTextRemoveStupidNewlines(manifestContent), null, Server.jsonDocumentOptions);
            if (node == null)
            {
                return -1;
            }
            ManifestRef? manifest = JsonSerializer.Deserialize<ManifestRef>(node);
            if (manifest == null)
            {
                Console.WriteLine("failed to parse manifest.json node");
                return -2;
            }
            if(manifest.Header == null)
            {
                Console.WriteLine("failed to get manifest header");
                return -3;
            }
            if (manifest.Header.Uuid == null)
            {
                Console.WriteLine("failed to get uuid");
                return -3;
            }
            if (manifest.Header.Version == null)
            {
                Console.WriteLine("failed to get version");
                return -3;
            }

            packRef = new PackRef {
                PackId = manifest.Header.Uuid,
                Version = manifest.Header.Version,
            };
            if(packRef == null)
            {
                return -3;
            }

            return 0;
        }
    }
}

