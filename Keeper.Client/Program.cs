using Keeper.Common;
using Keeper.Common.Enrichers;
using Serilog;
using Serilog.Core;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Keeper.Client
{
    public static class Program
    {
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private const int SW_HIDE = 0;
        private const int SW_SHOW = 5;

        [STAThread]
        public static void Main()
        {
            var handle = GetConsoleWindow();

            Console.Title = "DEBUG";

#if !DEBUG
            ShowWindow(handle, SW_HIDE);
#else
            ShowWindow(handle, SW_SHOW);
#endif

            Log.Logger = new LoggerConfiguration()
             .WriteTo.Async(log => log.File(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Client.log")))
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

            Console.CancelKeyPress += Events.OnCancelKeyPress;
            AppDomain.CurrentDomain.ProcessExit += OnExit;

            var Logger = Log.ForContext(Constants.SourceContextPropertyName, "Initialiazor");
            Logger.Information("Starting Client...");
            Client.Initialize();


            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            using (var loginForm = new LoginForm())
                Application.Run(loginForm);
        }

        private static void OnExit(object sender, EventArgs e)
        {
            Client.Instance?.Dispose();

            Log.CloseAndFlush();
        }
    }
}
