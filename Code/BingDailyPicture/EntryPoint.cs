namespace BingDailyPicture
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using Properties;

    static class EntryPoint
    {
        [STAThread]
        internal static void Main(string[] args)
        {
            UpgradeSettings();

            var vm = new MainWindowViewModel();

            if (args != null && args.Length > 0)
            {
                var parameters = args.Select(x => x.Trim().ToLowerInvariant());

                // Set the lock screen silently (e.g. from scheduled task)
                if (parameters.Any(x => string.Equals("/silent", x)))
                {
                    vm.ApplyLatestPicture().GetAwaiter().GetResult();
                    return;
                }
            }

            var window = new MainWindow(vm);
            var app = new App();
            app.Run(window);
        }

        static void UpgradeSettings()
        {
            try
            {
                // Upgrade / migrate custom settings if necessary
                Settings.Default.Upgrade();
            }
            catch (Exception ex)
            {
                Trace.TraceWarning("Couldn't upgrade settings: " + ex);
            }
        }
    }
}