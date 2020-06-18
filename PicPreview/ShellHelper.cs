using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace PicPreview
{
    public static class ShellHelper
    {
        // https://social.msdn.microsoft.com/Forums/office/en-US/d606a319-9357-4a41-994e-569ab055e1d5/api-to-get-shell-bags-value-hcrlocal-settingssoftwaremicrosoftwindowsshellbags-of-a-folder?forum=windowssdk
        // https://github.com/williballenthin/shellbags
        // https://microsoft.public.platformsdk.shell.narkive.com/HbbajK4y/how-retireve-sortdirection-from-property-bag
        // https://www.geoffchappell.com/studies/windows/shell/shlwapi/api/propbag/getviewstate.htm
        // https://docs.microsoft.com/en-us/windows/win32/api/shlwapi/nf-shlwapi-shgetviewstatepropertybag

        [DllImport("Shlwapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int SHGetViewStatePropertyBag(ref PCIDLIST_ABSOLUTE pidl, string pszBagName, int dwFlags, IntPtr riid, IntPtr ppv);
        // private static extern void SHGetViewStatePropertyBag(PCIDLIST_ABSOLUTE pidl, PCWSTR pszBagName, DWORD dwFlags, REFIID riid, IntPtr ppv);
        private struct PCIDLIST_ABSOLUTE
        {

        }

        public static void GetDirSortOrder(string dir)
        {
            PCIDLIST_ABSOLUTE pidl = new PCIDLIST_ABSOLUTE();
            int result = SHGetViewStatePropertyBag(ref pidl, dir, 0, IntPtr.Zero, IntPtr.Zero);
        }
    }
}
