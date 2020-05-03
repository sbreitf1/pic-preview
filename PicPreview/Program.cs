using System;
using System.Windows.Forms;

namespace PicPreview
{
    static class Program
    {
        public static string AppName
        {
            get
            {
                return System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            }
        }

        public static Version AppVersion
        {
            get
            {
                return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            }
        }

        public static string NiceString(this Version version)
        {
            string str = "v" + version.Major;
            int minor = version.Minor;
            int revision = version.Revision;
            int build = version.Build;
            if (minor != 0 || revision != 0 || build != 0)
            {
                str += "." + minor;
                if (revision != 0 || build !=0)
                {
                    str += "." + revision;
                    if (build!=0)
                    {
                        str += "." + build;
                    }
                }
            }
            return str;
        }


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
