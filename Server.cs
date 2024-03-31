using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;
using System.IO.Compression;
using System.Text.Json.Nodes;
using System.Xml.Linq;
using Microsoft.VisualBasic.FileIO;

namespace McBiboo
{
    public class Server
    {
        public static JsonDocumentOptions jsonDocumentOptions = new JsonDocumentOptions
        {
            CommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true,
        };

        public string ServerPath;
        public string CachePath;
        public string PacksPath;
        public string AddonsPath;
        public string WorldsPath;
        private string WorldPath;
        public string BehaviorPacksPath;
        public string ResourcePacksPath;
        public Dictionary<string, string> Properties;

        public string[] BehaviorPacks = { };
        public string[] ResourcePacks = { };

        public PackRef[] WorldResourcePacks = { };
        public PackRef[] WorldBehaviorPacks = { };

        public static readonly string[] DefaultBehaviorPacks = {
            "chemistry", "chemistry_1.20.50", "chemistry_1.20.60", "experimental_armadillo", "experimental_cameras", "experimental_update_announced_live2023",
            "experimental_villager_trade", "vanilla", "vanilla_1.14", "vanilla_1.15", "vanilla_1.16", "vanilla_1.16.100", "vanilla_1.16.200", "vanilla_1.16.210",
            "vanilla_1.16.220", "vanilla_1.17.0", "vanilla_1.17.10", "vanilla_1.17.20", "vanilla_1.17.30", "vanilla_1.17.40", "vanilla_1.18.0", "vanilla_1.18.10",
            "vanilla_1.18.20", "vanilla_1.18.30", "vanilla_1.19.0", "vanilla_1.19.10", "vanilla_1.19.20", "vanilla_1.19.30", "vanilla_1.19.40", "vanilla_1.19.50",
            "vanilla_1.19.60", "vanilla_1.19.70", "vanilla_1.19.80", "vanilla_1.20.0", "vanilla_1.20.10", "vanilla_1.20.20", "vanilla_1.20.30", "vanilla_1.20.40",
            "vanilla_1.20.50", "vanilla_1.20.60", "vanilla_1.20.70", "vanilla_1.20.72"
        };

        public static readonly string[] DefaultResourcePacks = {
            "chemistry", "vanilla"
        };

        public Server(string path)
        {
            Properties = new Dictionary<string, string>();
            ServerPath = path;
            CachePath = Path.Join(ServerPath, "cache");
            AddonsPath = Path.Join(ServerPath, "addons");
            PacksPath = Path.Join(ServerPath, "packs");
            WorldsPath = Path.Join(ServerPath, "worlds");
            BehaviorPacksPath = Path.Join(ServerPath, "behavior_packs");
            ResourcePacksPath = Path.Join(ServerPath, "resource_packs");
            VerifyMadeupFolders();
            GetBehaviorPacks();
            GetResourcePacks();
            GetProperties();
            if (Properties.ContainsKey("level-name"))
            {
                Console.WriteLine("got server properties");
                WorldPath = Path.Join(WorldsPath, Properties["level-name"]);
            } else
            {
                throw new Exception("Failed to get Server Properties");
            }
            GetAllRegisteredPacks();
        }

        private void GetWorldBehaviorPacks()
        {
            if (WorldPath == null)
            {
                Console.WriteLine("world path not yet set");
                return;
            }
            string WorldBehaviorPackPath = Path.Join(WorldPath, "world_behavior_packs.json");
            using (StreamReader r = new StreamReader(WorldBehaviorPackPath))
            {
                string json = r.ReadToEnd();
                List<PackRef>? items = JsonSerializer.Deserialize<List<PackRef>>(json);
                if(items == null)
                {
                    Console.WriteLine($"failed to get packs from: {WorldBehaviorPackPath}");
                    return;
                }
                WorldBehaviorPacks = items.ToArray();
            }
        }

        private void GetWorldResourcePacks()
        {
            if (WorldPath == null)
            {
                Console.WriteLine("world path not yet set");
                return;
            }
            string WorldResourcePackPath = Path.Join(WorldPath, "world_resource_packs.json");
            using (StreamReader r = new StreamReader(WorldResourcePackPath))
            {
                string json = r.ReadToEnd();
                List<PackRef>? items = JsonSerializer.Deserialize<List<PackRef>>(json);
                if (items == null)
                {
                    Console.WriteLine($"failed to get packs from: {WorldResourcePackPath}");
                    return;
                }
                WorldResourcePacks = items.ToArray();
            }
        }

        private void GetAllRegisteredPacks()
        {
            if (WorldPath == null)
            {
                Console.WriteLine("world path not yet set");
                return;
            }
            GetWorldBehaviorPacks();
            GetWorldResourcePacks();
        }

        private void VerifyMadeupFolders()
        {
            VerifyDirExists(CachePath);
            VerifyDirExists(AddonsPath);
            VerifyDirExists(PacksPath);
        }

        public void PrintPaths()
        {
            Console.WriteLine($"ServerPath {ServerPath}");
            Console.WriteLine($"CachePath {CachePath}");
            Console.WriteLine($"AddonsPath {AddonsPath}");
            Console.WriteLine($"WorldsPath {WorldsPath}");
            Console.WriteLine($"BehaviorPacksPath {BehaviorPacksPath}");
            Console.WriteLine($"ResourcePacksPath {ResourcePacksPath}");
        }

        public void GetProperties()
        {
            var lines = File.ReadAllLines(Path.Join(ServerPath, "server.properties"));
            foreach (string line in lines)
            {
                int comment_check = line.IndexOf('#');
                if (comment_check != -1 && comment_check < 4)
                {
                    continue;
                }
                int equal_check = line.IndexOf('=');
                if(equal_check == -1)
                {
                    continue;
                }
                string key = line.Substring(0, equal_check);
                string value = line.Substring(equal_check + 1);
                Properties[key] = value;
            }
        }

        private void VerifyDirExists(string path)
        {
            if (!Path.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        private void GetBehaviorPacks()
        {
            List<string> paths = new List<string>();
            foreach (var entry in Directory.GetDirectories(BehaviorPacksPath))
            {
                if (entry == null)
                {
                    continue;
                }
               // Console.WriteLine($"entry {entry}");
                var name = Path.GetFileName(entry);
                if (name == null)
                {
                    Console.WriteLine($"entry {entry}");
                    Console.WriteLine("failed to get directory name");
                    continue;
                }
                //Console.WriteLine($"entry name: {name}");
                if(DefaultBehaviorPacks.Contains(name))
                {
                    continue;
                }
                paths.Add(entry);
            }
            //Console.WriteLine($"paths: {paths.Count}");
            BehaviorPacks = paths.ToArray();
        }

        private void GetResourcePacks()
        {
            List<string> paths = new List<string>();
            foreach (var entry in Directory.GetDirectories(ResourcePacksPath))
            {
                if (entry == null)
                {
                    continue;
                }
                // Console.WriteLine($"entry {entry}");
                var name = Path.GetFileName(entry);
                if (name == null)
                {
                    Console.WriteLine($"entry {entry}");
                    Console.WriteLine("failed to get directory name");
                    continue;
                }
                //Console.WriteLine($"entry name: {name}");
                if (DefaultResourcePacks.Contains(name))
                {
                    continue;
                }
                paths.Add(entry);
            }
            //Console.WriteLine($"paths: {paths.Count}");
            ResourcePacks = paths.ToArray();
        }

        public void ClearCache()
        {
            foreach(var file in Directory.GetFiles(CachePath))
            {
                File.Delete(file);
            }
            foreach(var dir in Directory.GetDirectories(CachePath))
            {
                Directory.Delete(dir, true);
            }
        }

        public static string ReadStreamToString(Stream stream)
        {
            StreamReader reader = new StreamReader(stream);
            string output = reader.ReadToEnd();
            reader.Close();
            return output;
        }

        public int GetManifestJsonPackType(JsonNode? node)
        {
            ManifestRef? manifest = JsonSerializer.Deserialize<ManifestRef>(node);
            return GetManifestJsonPackType(manifest);
        }

        public int GetManifestJsonPackType(ManifestRef? manifest)
        {
            if (manifest == null)
            {
                Console.WriteLine("failed to parse manifest.json node");
                return -1;
            }
            if (manifest.Modules == null)
            {
                Console.WriteLine("failed to get manifest modules.");
                return -2;
            }
            foreach (ManifestModule module in manifest.Modules)
            {
                switch (module.Type)
                {
                    case "data":
                    case "script":
                        {
                            return 1;
                        };
                    case "resources":
                        {
                            return 2;
                        };
                    case "world_template":
                        {
                            return 3;
                        };
                    default:
                        {
                            Console.WriteLine($"got an unknown type string for one of the modules ({module.Type})");
                            return 4;
                        }
                }
            }
            return 0;
        }

        private void TryExtractToDirectory(ZipArchive archive, string outputDir)
        {
            try
            {
                archive.ExtractToDirectory(outputDir, true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private int IsPackAlreadyRegistered(ManifestRef manifest, int type)
        {
            if (manifest.Header == null)
            {
                Console.WriteLine("Failed to get manifest.json header in mcpack");
                return -1;
            }
            if (manifest.Header.Version == null)
            {
                Console.WriteLine("Failed to get manifest.json header version in mcpack");
                return -2;
            }
            if (manifest.Header.Uuid == null)
            {
                Console.WriteLine("Failed to get manifest.json header uuid in mcpack");
                return -2;
            }

            switch (type)
            {
                case 1:
                    {
                        foreach(PackRef pack in WorldBehaviorPacks)
                        {
                            if(pack.PackId == manifest.Header.Uuid)
                            {
                                if (pack.Version.SequenceEqual(manifest.Header.Version))
                                {
                                    return 1;
                                }
                            }
                        }
                    }; break;
                case 2:
                    {
                        foreach (PackRef pack in WorldResourcePacks)
                        {
                            if (pack.PackId == manifest.Header.Uuid)
                            {
                                if (pack.Version.SequenceEqual(manifest.Header.Version))
                                {
                                    return 1;
                                }
                            }
                        }
                    }; break;
                default:
                    {
                        Console.WriteLine($"failed to handle pack file got type return of {type}");
                    }; break;
            }
            return 0;
        }

        public void NewAddonHandleMcPack(string new_name_path, string name)
        {
            Console.WriteLine($"handling mcpack: {name}");
            var archive = ZipFile.Open(new_name_path, ZipArchiveMode.Read);
            if (archive == null)
            {
                Console.WriteLine("failed to open archive");
                return;
            }
            var manifest = archive.GetEntry("manifest.json");
            if (manifest == null)
            {
                Console.WriteLine("failed to find Zipfile Archive for manifest.json");
                return;
            }
            Stream stream = manifest.Open();
            string manifestContent = ReadStreamToString(stream);
            stream.Close();
            //Console.WriteLine($"manifestContent: {manifestContent}");


            var manifestNode = JsonObject.Parse(JsonTextRemoveStupidNewlines(manifestContent), null, jsonDocumentOptions);
            if (manifestNode == null)
            {
                Console.WriteLine("NewAddonHandleMcPack: failed to read json");
                return;
            }

            ManifestRef? manifestData = JsonSerializer.Deserialize<ManifestRef>(manifestNode);
            if(manifestData == null)
            {
                Console.WriteLine($"failed to get mcpack manifest.json: {name}");
                return;
            }

            int type = GetManifestJsonPackType(manifestData);
            int isPackAlreadyRegistered = IsPackAlreadyRegistered(manifestData, type);
            if (isPackAlreadyRegistered != 0)
            {
                if(isPackAlreadyRegistered < 0)
                {
                    Console.WriteLine("got error when checking if pack is already registered");
                    return;
                } else
                {
                    if(isPackAlreadyRegistered == 1)
                    {
                        archive.Dispose();
                        return;
                    }
                }
            }


            Console.WriteLine($"type: {type}");
            switch (type)
            {
                case 1:
                    {
                        var outputDir = Path.Join(BehaviorPacksPath, name);
                        VerifyDirExists(outputDir);
                        TryExtractToDirectory(archive, outputDir);
                    }; break;
                case 2:
                    {
                        var outputDir = Path.Join(ResourcePacksPath, name);
                        VerifyDirExists(outputDir);
                        TryExtractToDirectory(archive, outputDir);
                    }; break;
                default:
                    {
                        Console.WriteLine($"failed to handle pack file got type return of {type}");
                    }; break;
            }
            
            archive.Dispose();
        }

        public void NewAddonHandleMcAddon(string new_name_path)
        {
            Console.WriteLine("handling mcaddon");
            ZipArchive archive = ZipFile.Open(new_name_path, ZipArchiveMode.Read);
            List<(string, string)> mcPacks = new List<(string, string)>();
            foreach (var entry in archive.Entries)
            {
                //Console.WriteLine($"zip archive entry: {entry}");
                //Console.WriteLine(entry.Name);
                string entry_name = Path.GetFileNameWithoutExtension(entry.Name);
                string entry_extension = Path.GetExtension(entry.Name);
                //Console.WriteLine($"entry name: {entry_name}");
                switch (entry_extension)
                {
                    case ".mcpack":
                        {
                            string mcpack_entry_path = Path.Join(PacksPath, entry.Name);
                            try
                            {
                                entry.ExtractToFile(mcpack_entry_path, true);
                            } catch (Exception ex)
                            {
                                Console.WriteLine(ex.ToString());
                                continue;
                            }
                            mcPacks.Add((Path.Join(PacksPath, entry.Name), entry_name));
                        }; break;
                }
            }
            archive.Dispose();
            if (mcPacks.Count > 0)
            {
                foreach (var (path, name) in mcPacks)
                {
                    try
                    {
                        NewAddonHandleMcPack(path, name);
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine(ex.ToString());
                        break;
                    }
                }
            }
        }

        public static string JsonTextRemoveStupidNewlines(string text)
        {
            return text.Replace('\n', ' ');
        }

        public void NewAddon(string path)
        {
            string name = Path.GetFileNameWithoutExtension(path);
            string extension = Path.GetExtension(path);
            string? dir = Path.GetDirectoryName(path);
            Console.WriteLine($"{name} {extension}, {dir}");

            switch(extension)
            {
                case ".mcpack":
                    {
                        NewAddonHandleMcPack(path, name);
                    }; break;
                case ".mcaddon":
                    {
                        NewAddonHandleMcAddon(path);
                    }; break;
                default:
                    {
                        Console.WriteLine($"unhandled extension type {extension}");
                    }; break;
            }
        }

        public void CycleNewAddons(string path)
        {
            Console.WriteLine($"Cycling Dir: {path}");
            foreach(string entry in Directory.GetFiles(path))
            {
                try
                {
                    NewAddon(entry);
                } catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }

        public void Setup()
        {
            List<PackRef> resourcePacks = new List<PackRef>();
            List<PackRef> behaviorPacks = new List<PackRef>();
            foreach(var entry in ResourcePacks)
            {
                //Console.WriteLine($"{entry}");
                string manifestPath = Path.Join(entry, "manifest.json");
                if(!Path.Exists(manifestPath))
                {
                    Console.WriteLine($"manifest.json not found for {entry}");
                    continue;
                }
                string manifestContent = File.ReadAllText(manifestPath);
                PackRef? packRef = null;
                int retvalue = PackRefBase.FromManifestString(manifestContent, out packRef);
                if(packRef == null)
                {
                    Console.WriteLine($"failed to get pack ref from manifest.json in \"{entry}\"\ngot return value of {retvalue}");
                    continue;
                }
                resourcePacks.Add(packRef);
            }
            foreach (var entry in BehaviorPacks)
            {
                //Console.WriteLine($"{entry}");
                string manifestPath = Path.Join(entry, "manifest.json");
                if (!Path.Exists(manifestPath))
                {
                    Console.WriteLine($"manifest.json not found for {entry}");
                    continue;
                }
                string manifestContent = File.ReadAllText(manifestPath);
                PackRef? packRef = null;
                int retvalue = PackRefBase.FromManifestString(manifestContent, out packRef);
                if (packRef == null)
                {
                    Console.WriteLine($"failed to get pack ref from manifest.json in \"{entry}\"\ngot return value of {retvalue}");
                    continue;
                }
                behaviorPacks.Add(packRef);
            }
            PackRef[] resourcePacksArray = resourcePacks.ToArray();
            PackRef[] behaviorPacksArray = behaviorPacks.ToArray();
            string jsonResourcePacks = JsonSerializer.Serialize(resourcePacksArray);
            string jsonBehaviorPacks = JsonSerializer.Serialize(behaviorPacksArray);
            File.WriteAllText(Path.Join(WorldPath, "world_resource_packs.json"), jsonResourcePacks);
            File.WriteAllText(Path.Join(WorldPath, "world_behavior_packs.json"), jsonBehaviorPacks);
        }
    }
}
