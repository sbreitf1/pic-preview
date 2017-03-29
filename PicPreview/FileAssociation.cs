using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace PicPreview
{
    static class FileAssociation
    {
        private const string ProgId = "sbreitf1.PicPreview.image";

        [DllImport("Shell32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern void SHChangeNotify(int wEventId, uint uFlags, IntPtr dwItem1, IntPtr dwItem2);
        private const int SHCNE_ASSOCCHANGED = 0x8000000;
        private const int SHCNF_IDLIST = 0;
        private const int SHCNF_FLUSHNOWAIT = 0x2000;


        static FileAssociation()
        {
            SHChangeNotify(SHCNE_ASSOCCHANGED, SHCNF_IDLIST | SHCNF_FLUSHNOWAIT, IntPtr.Zero, IntPtr.Zero);
        }


        public static bool ProgIdExists()
        {
            try
            {
                RegistryKey key = Registry.ClassesRoot.OpenSubKey(ProgId);
                bool result = (key != null);
                if (key != null)
                    key.Close();
                return result;
            }
            catch { return false; }
        }
        public static bool CreateProgId(bool elevate = true)
        {
            try
            {
                if (elevate)
                {
                    RunCommands("CreateProgId");
                    return ProgIdExists();
                }
                else
                {
                    RegistryKey key = Registry.ClassesRoot.CreateSubKey(ProgId);
                    key.SetValue("", "PicPreview");
                    //key.SetValue("FriendlyTypeName", "BILD!");

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
                    return true;
                }
            }
            catch { return false; }
        }
        public static bool DeleteProgId(bool elevate = true)
        {
            try
            {
                if (elevate)
                {
                    RunCommands("DeleteProgId");
                    return !ProgIdExists();
                }
                else
                {
                    Registry.ClassesRoot.DeleteSubKeyTree(ProgId);
                    return true;
                }
            }
            catch { return false; }
        }


        public static bool ApplicationIsRegistered()
        {
            try
            {
                RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Clients\Media\PicPreview");
                bool result = (key != null);
                if (key != null)
                    key.Close();
                return result;
            }
            catch { return false; }
        }
        public static bool RegisterApplication(bool elevate = true)
        {
            try
            {
                if (elevate)
                {
                    RunCommands("CreateProgId", "RegisterApplication");
                    SHChangeNotify(SHCNE_ASSOCCHANGED, SHCNF_IDLIST | SHCNF_FLUSHNOWAIT, IntPtr.Zero, IntPtr.Zero);
                    return ApplicationIsRegistered();
                }
                else
                {
                    SetupAppRegistration();
                    SetupExecutableRegistration();
                    return true;
                }
            }
            catch { return false; }
        }
        private static void SetupAppRegistration()
        {
            RegistryKey key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Clients\Media\PicPreview");
            key.SetValue("", "PicPreview");

            RegistryKey capabilitiesKey = key.CreateSubKey("Capabilities");
            capabilitiesKey.SetValue("ApplicationDescription", "Advanced preview tool for images and animations.");
            capabilitiesKey.SetValue("ApplicationName", "PicPreview");

            RegistryKey assocKey = capabilitiesKey.CreateSubKey("FileAssociations");
            assocKey.SetValue(".jpg", ProgId);
            assocKey.SetValue(".jpeg", ProgId);

            RegistryKey installInfoKey = key.CreateSubKey("InstallInfo");
            installInfoKey.SetValue("ReinstallCommand", "\"C:\\Program Files (x86)\\VideoLAN\\VLC\\spad-setup.exe\" /Reinstall /S");
            installInfoKey.SetValue("ShowIconsCommand", "\"C:\\Program Files (x86)\\VideoLAN\\VLC\\spad-setup.exe\" /ShowIcons /S");
            installInfoKey.SetValue("HideIconsCommand", "\"C:\\Program Files (x86)\\VideoLAN\\VLC\\spad-setup.exe\" /HideIcons /S");
            installInfoKey.SetValue("IconsVisible", 1);

            RegistryKey appsKey = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\RegisteredApplications");
            appsKey.SetValue("PicPreview", @"SOFTWARE\Clients\Media\PicPreview\Capabilities");
        }
        private static void SetupExecutableRegistration()
        {
            RegistryKey key = Registry.ClassesRoot.CreateSubKey(@"Applications\" + Path.GetFileName(Environment.GetCommandLineArgs()[0]));

            RegistryKey shellKey = key.CreateSubKey("shell");
            shellKey.SetValue("", "open");
            RegistryKey openKey = shellKey.CreateSubKey("open");
            openKey.SetValue("", "Vorschau");
            RegistryKey previewCommandKey = openKey.CreateSubKey("command");

            RegistryKey typesKey = key.CreateSubKey("SupportedTypes");
            typesKey.SetValue(".jpg", ProgId);
            typesKey.SetValue(".jpeg", ProgId);
            
            previewCommandKey.SetValue("", "\"" + Environment.GetCommandLineArgs()[0] + "\" \"%1\"");
        }
        public static bool UnregisterApplication(bool elevate = true)
        {
            try
            {
                if (elevate)
                {
                    RunCommands("UnregisterApplication");
                    SHChangeNotify(SHCNE_ASSOCCHANGED, SHCNF_IDLIST | SHCNF_FLUSHNOWAIT, IntPtr.Zero, IntPtr.Zero);
                    return !ApplicationIsRegistered();
                }
                else
                {
                    Registry.LocalMachine.DeleteSubKeyTree(@"SOFTWARE\Clients\Media\PicPreview");
                    return true;
                }
            }
            catch { return false; }
        }


        public static bool IsAssociated(string[] extensions)
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
        }


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
