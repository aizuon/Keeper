using Hjson;
using Keeper.Json.Converters;
using Keeper.Server.Database;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;

namespace Keeper.Server
{
    public class Config
    {
        private static readonly string s_path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Server.hjson");

        static Config()
        {
            if (!File.Exists(s_path))
            {
                Instance = new Config();
                Instance.Save();
                return;
            }

            using (var fs = new FileStream(s_path, FileMode.Open, FileAccess.Read))
            {
                Instance = JsonConvert.DeserializeObject<Config>(HjsonValue.Load(fs).ToString(Stringify.Plain));
            }
        }

        public Config()
        {
            Listener = new IPEndPoint(IPAddress.Loopback, 1337);
            MaxConnections = 10;
            ConnectionKey = string.Empty;
            PrivateKey = string.Empty;
            Database = new DatabasesConfig();
        }

        public static Config Instance { get; }

        [JsonProperty("listener")]
        [JsonConverter(typeof(IPEndPointConverter))]
        public IPEndPoint Listener { get; set; }
        [JsonProperty("max_connections")]
        public uint MaxConnections { get; set; }
        [JsonProperty("connection_key")]
        public string ConnectionKey { get; set; }
        [JsonProperty("private_key")]
        public string PrivateKey { get; set; }

        [JsonProperty("database")]
        public DatabasesConfig Database { get; set; }

        public void Save()
        {
            string json = JsonConvert.SerializeObject(this, Formatting.None);
            File.WriteAllText(s_path, JsonValue.Parse(json).ToString(Stringify.Hjson));
        }
    }
}
