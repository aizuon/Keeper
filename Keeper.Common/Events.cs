using Serilog;
using Serilog.Core;
using System;
using System.Threading.Tasks;

namespace Keeper.Common
{
    public static class Events
    {
        private static readonly ILogger Logger = Log.ForContext(Constants.SourceContextPropertyName, nameof(Events));

        public static void OnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            e.SetObserved();

            Logger.Error(e.Exception.ToString());
        }

        public static void OnUnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            Logger.Error(((Exception)args.ExceptionObject).ToString());

            Environment.Exit(-1);
            return;
        }

        public static void OnCancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            Environment.Exit(0);
            return;
        }
    }
}
