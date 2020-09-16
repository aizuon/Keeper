using Dapper;
using Dapper.FastCrud;
using Microsoft.Data.Sqlite;
using MySql.Data.MySqlClient;
using Serilog;
using Serilog.Core;
using System;
using System.Data;
using System.IO;
using System.Net;


namespace Keeper.Server.Database
{
    public static class DB
    {
        private static readonly ILogger Logger = Log.ForContext(Constants.SourceContextPropertyName, nameof(DB));

        private static DatabasesConfig _config;
        private static string s_connectionString;

        public static void Initialize(DatabasesConfig config)
        {
            if (s_connectionString != null)
                throw new InvalidOperationException("Database is already initialized");

            _config = config;

            switch (config.Engine)
            {
                case DatabaseEngine.MySQL:
                    s_connectionString =
                        $"SslMode=none;Server={config.Info.Host};Port={config.Info.Port};Database={config.Info.Database};Uid={config.Info.Username};Pwd={config.Info.Password};Pooling=true;";
                    OrmConfiguration.DefaultDialect = SqlDialect.MySql;

                    using (var con = Open())
                    {
                        if (con.QueryFirstOrDefault($"SHOW DATABASES LIKE \"{config.Info.Database}\"") == null)
                        {
                            Logger.Error("Database {0} not found!", config.Info.Database);
                            Environment.Exit(0);
                            return;
                        }
                    }

                    break;

                case DatabaseEngine.SQLite:
                    s_connectionString = $"Data Source={config.Info.Filename};Pooling=true;";
                    OrmConfiguration.DefaultDialect = SqlDialect.SqLite;

                    if (!File.Exists(config.Info.Filename))
                    {
                        Logger.Error("Database {0} not found!", config.Info.Filename);
                        Environment.Exit(0);
                        return;
                    }

                    break;

                default:
                    Logger.Error("Invalid database engine {0}", config.Engine);
                    Environment.Exit(0);
                    return;
            }

            Logger.Information("Connected to database on {0}", new IPEndPoint(IPAddress.Parse(config.Info.Host), (int)config.Info.Port).ToString());
        }

        public static IDbConnection Open()
        {
            if (s_connectionString == null)
            {
                Logger.Error("Database should be initialized first!");

                Environment.Exit(0);
                return null;
            }

            var engine = _config.Engine;
            IDbConnection connection;
            switch (engine)
            {
                case DatabaseEngine.MySQL:
                    connection = new MySqlConnection(s_connectionString);
                    break;

                case DatabaseEngine.SQLite:
                    connection = new SqliteConnection(s_connectionString);
                    break;

                default:
                    Logger.Error("Invalid database engine {0}", engine);
                    Environment.Exit(0);
                    return null;
            }

            connection.Open();
            return connection;
        }
    }
}
