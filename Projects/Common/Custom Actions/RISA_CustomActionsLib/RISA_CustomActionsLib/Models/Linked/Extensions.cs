using System;
using System.Diagnostics;
using System.Linq;
using System.Management;

namespace RISA_CustomActionsLib.Models.Linked
{
    #region Helper classes


    // BootstrapperData has its own Extensions class, to be self contained,
    //  as BootstrapperData.cs is linked to the Silent-PreInstall project
    //
    public static partial class Extensions
    {
        public static string Dq(this string src)
        {
            const string ddq = @"""";
            return $"{ddq}{src}{ddq}";
        }
        public static string EnsureTrailingBash(this string path)
        {
            const string bash = @"\";
            if (string.IsNullOrEmpty(path)) return path;
            if (path.Substring(path.Length - 1, 1) == bash) return path;
            return path += bash;
        }
        public static bool IsEqIgnoreCase(this string src, string cmpWith)
        {
            return string.Compare(src, cmpWith, StringComparison.CurrentCultureIgnoreCase) == 0;
        }

        public static string ToDetailStr(this string src)
        {
            if (src == null) return "null";
            if (src.Length == 0) return "empty";
            return src;
        }

        public static Version ToVersion(this string versionStr)
        {
            //normalize inpVersion so it has 4 parts
            var inv = versionStr;
            while (inv.EndsWith(".")) inv = inv.Substring(0, inv.Length - 1);
            if (inv.Length == 0) inv = "1";
            var versionPartsCt = inv.Split('.').Length;
            for (var i = versionPartsCt; i < 4; i++) inv += ".0";
            // excp if some part of this does not parse to an int:
            return new Version(inv);
        }

        // Silent Install extensions

        public static Process GetParent(this Process process)
        {
            try
            {
                using (var query = new ManagementObjectSearcher(
                    "SELECT * " +
                    "FROM Win32_Process " +
                    "WHERE ProcessId=" + process.Id))
                {
                    return query
                        .Get()
                        .OfType<ManagementObject>()
                        .Select(p => Process.GetProcessById((int)(uint)p["ParentProcessId"]))
                        .FirstOrDefault();
                }
            }
            catch
            {
                return null;
            }
        }
        public static string GetCommandLine(this Process process)
        {
            var queryStr = $"SELECT CommandLine FROM Win32_Process WHERE ProcessId={process.Id}";
            try
            {
                using (var searcher = new ManagementObjectSearcher(queryStr))
                using (var objects = searcher.Get())
                {
                    return objects.Cast<ManagementBaseObject>().SingleOrDefault()?["CommandLine"]?.ToString();
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public static string GetExecutablePath(this Process process)
        {
            var queryStr = $"SELECT ExecutablePath FROM Win32_Process WHERE ProcessId={process.Id}";
            using (var searcher = new ManagementObjectSearcher(queryStr))
            using (var objects = searcher.Get())
            {
                return objects.Cast<ManagementBaseObject>().SingleOrDefault()?["ExecutablePath"]?.ToString();
            }
        }

    }

    #endregion


}
