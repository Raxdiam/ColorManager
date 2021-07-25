using System;
using System.IO;
using System.Windows.Forms;

namespace ColorManager
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            if (!Directory.Exists(AppData)) Directory.CreateDirectory(AppData);
            Config = Config.Load(Path.Combine(AppData, "config.json"));

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }

        public static readonly string AppData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ColorManager");

        public static Config Config { get; private set; }
    }
}
