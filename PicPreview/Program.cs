﻿using System;
using System.Windows.Forms;

namespace PicPreview
{
    static class Program
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

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
            int build = version.Build;
            int revision = version.Revision;
            if (minor != 0 || build != 0 || revision != 0)
            {
                str += "." + minor;
                if (build != 0 || revision != 0)
                {
                    str += "." + build;
                    if (revision != 0)
                    {
                        str += "." + revision;
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
            logger.Info("Application startup");
            AppDomain.CurrentDomain.UnhandledException += HandleException;

            //ShellHelper.GetDirSortOrder(@"G:\Eigene Dateien\Downloads");

            /*Microsoft.WindowsAPICodePack.Shell.ShellFile f = Microsoft.WindowsAPICodePack.Shell.ShellFile.FromFilePath(@"G:\Eigene Dateien\Eigene Bilder\Wallpaper\epic-sunset-wallpapers_6757_1280x800.jpg");
            System.Drawing.Bitmap b = f.Thumbnail.LargeBitmap;
            b.Save(@"G:\backup-api\thumb.png");
            b.Dispose();
            return;*/

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (Properties.Settings.Default.FirstStart)
            {
                logger.Info("Setup for first start");
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.FirstStart = false;
                Properties.Settings.Default.Save();
            }

            Application.Run(new MainForm());

            logger.Info("Application shut down");
        }

        private static void HandleException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            if (ex != null)
            {
                logger.Error("Unhandled " + ex.GetType().Name + ": " + ex.Message);
                logger.Error(ex.StackTrace);
            }
            else
            {
                logger.Error("Unknown error in global context");
            }
            //TODO message box
        }
    }
}
