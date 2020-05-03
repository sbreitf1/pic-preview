using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace PicPreview
{
    class FileAssociationHelper
    {
        [DllImport("Shell32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern void SHChangeNotify(int wEventId, uint uFlags, IntPtr dwItem1, IntPtr dwItem2);
        private const int SHCNE_ASSOCCHANGED = 0x8000000;
        private const int SHCNF_IDLIST = 0;
        private const int SHCNF_FLUSHNOWAIT = 0x2000;

        /// <summary>
        /// Returns whether the user needs to be prompted to change the system default application for images.
        /// 
        /// Use ShowSystemDefaultsPanel instead of AssociateFileExtensionsForUser if true is returned.
        /// </summary>
        public bool NeedUserPrompt
        {
            get
            {
                // since Windows 10 the user has to select the default by hand
                return Environment.OSVersion.Version.Major > 6;
            }
        }

        private string appName;
        private string visibleName;
        private string progId;
        private string[] extensions;


        public FileAssociationHelper(string appName, string visibleName, string progId, string[] extensions)
        {
            this.appName = appName;
            this.visibleName = visibleName;
            this.progId = progId;
            this.extensions = extensions;
        }


        private bool ProgIdExistsForUser()
        {
            try
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Classes\" + this.progId);
                bool result = (key != null);
                if (key != null)
                    key.Close();
                return result;
            }
            catch { return false; }
        }
        private void CreateProgIdForUser()
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey(@"Software\Classes\" + this.progId);
            key.SetValue("", this.visibleName);

            RegistryKey defaultIconKey = key.CreateSubKey("DefaultIcon");
            defaultIconKey.SetValue("", @"%SystemRoot%\System32\imageres.dll,-72");

            RegistryKey curVerKey = key.CreateSubKey("CurVer");
            curVerKey.SetValue("", this.progId);

            RegistryKey shellKey = key.CreateSubKey("shell");
            shellKey.SetValue("", "open");
            RegistryKey openKey = shellKey.CreateSubKey("open");
            openKey.SetValue("", "Vorschau");
            RegistryKey previewCommandKey = openKey.CreateSubKey("command");
            previewCommandKey.SetValue("", "\"" + Environment.GetCommandLineArgs()[0] + "\" \"%1\"");
        }
        private void DeleteProgIdForUser()
        {
            Registry.CurrentUser.DeleteSubKeyTree(@"Software\Classes\" + this.progId);
        }


        /// <summary>
        /// Registers the application in the system so it is visible in the defaults selection panel and has a known ProgID class.
        /// </summary>
        /// <returns></returns>
        public bool ApplicationIsRegisteredForUser()
        {
            try
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\" + this.appName);
                bool result = (key != null);
                if (key != null)
                    key.Close();
                return result && ProgIdExistsForUser();
            }
            catch { return false; }
        }
        public void RegisterApplicationForUser()
        {
            CreateProgIdForUser();

            RegistryKey key = Registry.CurrentUser.CreateSubKey(@"Software\" + this.appName);
            key.SetValue("", this.visibleName);

            RegistryKey capabilitiesKey = key.CreateSubKey("Capabilities");
            capabilitiesKey.SetValue("ApplicationDescription", "Advanced preview tool for images and animations.");
            capabilitiesKey.SetValue("ApplicationName", this.visibleName);

            RegistryKey assocKey = capabilitiesKey.CreateSubKey("FileAssociations");
            foreach (string ext in this.extensions)
            {
                assocKey.SetValue(ext, this.progId);
            }

            RegistryKey appsKey = Registry.CurrentUser.CreateSubKey(@"Software\RegisteredApplications");
            appsKey.SetValue(this.appName, @"Software\"+ this.appName+ @"\Capabilities");

            SHChangeNotify(SHCNE_ASSOCCHANGED, SHCNF_IDLIST | SHCNF_FLUSHNOWAIT, IntPtr.Zero, IntPtr.Zero);
        }
        public void UnregisterApplicationForUser()
        {
            Registry.CurrentUser.DeleteSubKeyTree(@"Software\" + this.appName);
            Registry.CurrentUser.OpenSubKey(@"Software\RegisteredApplications").DeleteValue(this.appName);
            DeleteProgIdForUser();
            SHChangeNotify(SHCNE_ASSOCCHANGED, SHCNF_IDLIST | SHCNF_FLUSHNOWAIT, IntPtr.Zero, IntPtr.Zero);
        }


        /// <summary>
        /// Can be used to automatically associate all file extensions with the application. This method can only be used, if NeedUserPrompt returns false. Must be used after RegisterApplicationForUser.
        /// </summary>
        public void AssociateFileExtensionsForUser()
        {
            RegistryKey classesKey = Registry.CurrentUser.CreateSubKey(@"Software\Classes");
            foreach (string ext in this.extensions)
            {
                classesKey.CreateSubKey(ext).SetValue("", this.progId);
            }
            SHChangeNotify(SHCNE_ASSOCCHANGED, SHCNF_IDLIST | SHCNF_FLUSHNOWAIT, IntPtr.Zero, IntPtr.Zero);
        }


        /// <summary>
        /// Displays the system default selection panel to the user. This method should be used, if NeedUserPrompt returns true. Must be used after RegisterApplicationForUser.
        /// </summary>
        public void ShowSystemDefaultsPanel()
        {
            //TODO switch for older systems to just set file extension in ClassesRoot
            Process.Start(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), @"system32\control.exe"), "/name Microsoft.DefaultPrograms /page pageDefaultProgram");
        }
    }
}
