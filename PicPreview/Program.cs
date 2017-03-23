using System;
using System.Windows.Forms;

namespace PicPreview
{
    static class Program
    {
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if(Properties.Settings.Default.FirstStart)
            {
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.FirstStart = false;
                Properties.Settings.Default.Save();
            }

            Application.Run(new MainForm());
        }
    }
}
