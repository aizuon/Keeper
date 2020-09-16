using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Keeper.Server.Database
{
    public enum DatabaseEngine
    {
        SQLite,
        MySQL,
        PostgreSQL
    }

    public class DatabasesConfig
    {
        public DatabasesConfig()
        {
            Engine = DatabaseEngine.MySQL;
            Info = new DatabaseConfig { Filename = string.Empty };
        }

        [JsonProperty("engine")]
        [JsonConverter(typeof(StringEnumConverter))]
        public DatabaseEngine Engine { get; set; }

        [JsonProperty("info")]
        public DatabaseConfig Info { get; set; }

        public class DatabaseConfig
        {
            public DatabaseConfig()
            {
                Host = "127.0.0.1";
                Port = 3306;
            }

            [JsonProperty("filename")]
            public string Filename { get; set; }

            [JsonProperty("host")]
            public string Host { get; set; }

            [JsonProperty("port")]
            public uint Port { get; set; }

            [JsonProperty("username")]
            public string Username { get; set; }

            [JsonProperty("password")]
            public string Password { get; set; }

            [JsonProperty("database")]
            public string Database { get; set; }
        }
    }
}
