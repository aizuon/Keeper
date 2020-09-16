using Keeper.Common;
using Keeper.Common.Enrichers;
using Keeper.Server.Database;
using Serilog;
using Serilog.Core;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Keeper.Server
{
    public static class Program
    {
        public static async Task Main()
        {
            Log.Logger = new LoggerConfiguration()
             .WriteTo.Async(log => log.File(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Server.log")))
             .WriteTo.Async(console => console.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] |{SrcContext}| {Message}{NewLine}{Exception}"))
             .Enrich.With<ContextEnricher>()
#if DEBUG
                         .MinimumLevel.Verbose()
#else
                         .MinimumLevel.Information()
#endif
                         .CreateLogger();

            AppDomain.CurrentDomain.UnhandledException += Events.OnUnhandledException;
            TaskScheduler.UnobservedTaskException += Events.OnUnobservedTaskException;

            var Logger = Log.ForContext(Constants.SourceContextPropertyName, "Initialiazor");

            Logger.Information("Initializing Database...");
            DB.Initialize(Config.Instance.Database);

            Logger.Information("Starting Hub Server...");
            Server.Initialize();
            Server.Instance.Listen(Config.Instance.Listener);

            Console.CancelKeyPress += Events.OnCancelKeyPress;
            AppDomain.CurrentDomain.ProcessExit += OnExit;

            await Task.Delay(Timeout.Infinite);
        }

        private static void OnExit(object sender, EventArgs e)
        {
            Server.Instance.Dispose();

            Log.CloseAndFlush();
        }
    }
}
