using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace PicPreview
{
    static class FileAssociation
    {
        private const string ProgId = "sbreitf1.PicPreview.image";
        private const string AppVisibleName = "PicPreview";

        [DllImport("Shell32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern void SHChangeNotify(int wEventId, uint uFlags, IntPtr dwItem1, IntPtr dwItem2);
        private const int SHCNE_ASSOCCHANGED = 0x8000000;
        private const int SHCNF_IDLIST = 0;
        private const int SHCNF_FLUSHNOWAIT = 0x2000;


        private static bool ProgIdExistsForUser()
        {
            try
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Classes\" + ProgId);
                bool result = (key != null);
                if (key != null)
                    key.Close();
                return result;
            }
            catch { return false; }
        }
        private static void CreateProgIdForUser()
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey(@"Software\Classes\" + ProgId);
            key.SetValue("", AppVisibleName);

            RegistryKey defaultIconKey = key.CreateSubKey("DefaultIcon");
            defaultIconKey.SetValue("", @"%SystemRoot%\System32\imageres.dll,-72");

            RegistryKey curVerKey = key.CreateSubKey("CurVer");
            curVerKey.SetValue("", ProgId);

            RegistryKey shellKey = key.CreateSubKey("shell");
            shellKey.SetValue("", "open");
            RegistryKey openKey = shellKey.CreateSubKey("open");
            openKey.SetValue("", "Vorschau");
            RegistryKey previewCommandKey = openKey.CreateSubKey("command");
            previewCommandKey.SetValue("", "\"" + Environment.GetCommandLineArgs()[0] + "\" \"%1\"");
        }
        private static void DeleteProgIdForUser()
        {
            Registry.CurrentUser.DeleteSubKeyTree(@"Software\Classes\" + ProgId);
        }


        public static bool ApplicationIsRegisteredForUser()
        {
            try
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\PicPreview");
                bool result = (key != null);
                if (key != null)
                    key.Close();
                return result && ProgIdExistsForUser();
            }
            catch { return false; }
        }
        public static void RegisterApplicationForUser()
        {
            CreateProgIdForUser();

            RegistryKey key = Registry.CurrentUser.CreateSubKey(@"Software\PicPreview");
            key.SetValue("", "PicPreview");

            RegistryKey capabilitiesKey = key.CreateSubKey("Capabilities");
            capabilitiesKey.SetValue("ApplicationDescription", "Advanced preview tool for images and animations.");
            capabilitiesKey.SetValue("ApplicationName", AppVisibleName);

            RegistryKey assocKey = capabilitiesKey.CreateSubKey("FileAssociations");
            assocKey.SetValue(".bmp", ProgId);
            assocKey.SetValue(".gif", ProgId);
            assocKey.SetValue(".jpeg", ProgId);
            assocKey.SetValue(".jpg", ProgId);
            assocKey.SetValue(".png", ProgId);
            assocKey.SetValue(".tif", ProgId);
            assocKey.SetValue(".tiff", ProgId);
            assocKey.SetValue(".tga", ProgId);
            assocKey.SetValue(".webp", ProgId);

            RegistryKey appsKey = Registry.CurrentUser.CreateSubKey(@"Software\RegisteredApplications");
            appsKey.SetValue(AppVisibleName, @"Software\PicPreview\Capabilities");

            SHChangeNotify(SHCNE_ASSOCCHANGED, SHCNF_IDLIST | SHCNF_FLUSHNOWAIT, IntPtr.Zero, IntPtr.Zero);
        }
        public static void UnregisterApplicationForUser()
        {
            Registry.CurrentUser.DeleteSubKeyTree(@"Software\PicPreview");
            Registry.CurrentUser.OpenSubKey(@"Software\RegisteredApplications").DeleteValue(AppVisibleName);
            DeleteProgIdForUser();
            SHChangeNotify(SHCNE_ASSOCCHANGED, SHCNF_IDLIST | SHCNF_FLUSHNOWAIT, IntPtr.Zero, IntPtr.Zero);
        }


        public static void AssociateForUser()
        {
            //TODO switch for older systems to just set file extension in ClassesRoot
            Process.Start(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), @"system32\control.exe"), "/name Microsoft.DefaultPrograms /page pageDefaultProgram");
        }


        /*public static bool IsAssociated(string[] extensions)
        {
            try
            {
                for (int i = 0; i < extensions.Length; i++)
                {
                    if (!extensions[i].StartsWith("."))
                        extensions[i] = "." + extensions[i];

                    RegistryKey key = Registry.ClassesRoot.OpenSubKey(extensions[i]);
                    if (key == null)
                        return false;

                    string val = (key.GetValue("") as string);
                    if (string.IsNullOrWhiteSpace(val))
                        return false;

                    if (!val.Equals(ProgId))
                        return false;
                }
                return true;
            }
            catch { return false; }
        }
        public static bool IsAssociated(string ext)
        {
            return IsAssociated(new string[] { ext });
        }

        public static bool Associate(string[] extensions, bool elevate = true)
        {
            try
            {
                for (int i = 0; i < extensions.Length; i++)
                    if (!extensions[i].StartsWith("."))
                        extensions[i] = "." + extensions[i];

                if (elevate)
                {
                    List<string> commands = new List<string>(2 + extensions.Length);
                    commands.Add("CreateProgId");
                    commands.Add("RegisterApplication");
                    foreach (string ext in extensions)
                        commands.Add("Associate:" + ext);
                    RunCommands(commands.ToArray());

                    SHChangeNotify(SHCNE_ASSOCCHANGED, SHCNF_IDLIST | SHCNF_FLUSHNOWAIT, IntPtr.Zero, IntPtr.Zero);

                    if (!ProgIdExists())
                        return false;
                    foreach (string ext in extensions)
                        if (!IsAssociated(ext))
                            return false;
                    return true;
                }
                else
                {
                    foreach (string ext in extensions)
                    {
                        RegistryKey key = Registry.ClassesRoot.CreateSubKey(ext);
                        key.SetValue("", ProgId);
                    }
                    return true;
                }
            }
            catch { return false; }
        }
        public static bool Associate(string ext, bool elevate = true)
        {
            return Associate(new string[] { ext }, elevate);
        }*/


        private static bool RunCommands(params string[] commands)
        {
            StringBuilder sb = new StringBuilder("-cmd ");
            foreach (string cmd in commands)
            {
                sb.Append("\"").Append(cmd).Append("\" ");
            }
            ProcessStartInfo psi = new ProcessStartInfo(Environment.GetCommandLineArgs()[0], sb.ToString());
            psi.Verb = "runas";
            Process p = Process.Start(psi);
            p.WaitForExit();
            return true;
        }
    }
}
