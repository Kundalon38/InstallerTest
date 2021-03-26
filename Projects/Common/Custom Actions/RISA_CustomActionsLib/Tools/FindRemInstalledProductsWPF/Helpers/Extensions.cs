
using System;
using System.CodeDom;

namespace FindRemInstalledProductsWPF.Helpers
{
    public static partial class Extensions
    {
        public static string Cterm(this string src)
        {
            const string c = ",";
            return src == null ? c : $"{src}{c}";
        }
        public static string Dq(this string src)
        {
            const string ddq = @"""";
            return $"{ddq}{src}{ddq}";
        }
        public static string DqC(this string src)
        {
            const string ddq = @"""";
            const string c = ",";
            return src == null ? c : $"{ddq}{src}{ddq}{c}";
        }

        public static string ToYN(this bool src)
        {
            return src ? "Y" : "N";
        }

        public static string ToStr(this int? src)
        {
            return src.HasValue ? src.Value.ToString() : string.Empty;
        }

        public static string EnsureTrailingBash(this string path)
        {
            const string bash = @"\";
            if (string.IsNullOrEmpty(path)) return path;
            if (path.Substring(path.Length - 1, 1) == bash) return path;
            return path += bash;
        }

        public static Version ToVersion(this string versionStr)
        {
            //normalize inpVersion so it has 4 parts
            var inv = versionStr;
            while (inv.EndsWith(".")) inv = inv.Substring(0, inv.Length - 1);
            if (inv.Length == 0) inv = "1";
            var versionPartsCt = inv.Split('.').Length;
            for (var i = versionPartsCt; i < 4; i++) inv += ".0";
            return new Version(inv);
        }

        public static Version ToVersion2(this string versionStr)
        {
            //normalize inpVersion so it has 4 parts
            var inv = versionStr;
            while (inv.EndsWith(".")) inv = inv.Substring(0, inv.Length - 1);
            if (inv.Length == 0) inv = "1";
            var versionPartsCt = inv.Split('.').Length;
            for (var i = versionPartsCt; i < 2; i++) inv += ".0";
            return new Version(inv);
        }

        public static int VersionPartsCount(this string versionStr)
        {
            var inv = versionStr;
            while (inv.EndsWith(".")) inv = inv.Substring(0, inv.Length - 1);
            if (inv.Length == 0) inv = "1";
            return inv.Split('.').Length;
        }
    }
}
