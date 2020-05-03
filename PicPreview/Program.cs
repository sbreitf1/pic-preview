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
            /*string[] args = Environment.GetCommandLineArgs();
            if (args.Length >= 2 && args[1] == "-cmd")
            {
                for (int i = 2; i < args.Length; i++)
                    ExecuteCommand(args[i]);
                return;
            }*/

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (Properties.Settings.Default.FirstStart)
            {
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.FirstStart = false;
                Properties.Settings.Default.Save();
            }

            Application.Run(new MainForm());
        }


        /*static void ExecuteCommand(string cmd)
        {
            if (cmd.StartsWith("CreateProgId"))
                FileAssociation.CreateProgId(false);
            else if (cmd.StartsWith("DeleteProgId"))
                FileAssociation.DeleteProgId(false);
            else if (cmd.StartsWith("RegisterApplication"))
                FileAssociation.RegisterApplication(false);
            else if (cmd.StartsWith("UnregisterApplication"))
                FileAssociation.UnregisterApplication(false);
            else if (cmd.StartsWith("Associate:"))
                FileAssociation.Associate(cmd.Substring(10), false);
        }*/
    }
}
