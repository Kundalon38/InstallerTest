using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace namestamp
{
    class Program
    {
        static int Main(string[] args)
        {
            try
            {
                string fileName = args[0];

                var versInfo = FileVersionInfo.GetVersionInfo(fileName);
                int major = versInfo.FileMajorPart;
                int minor = versInfo.FileMinorPart;
                int patch = versInfo.FileBuildPart;
                int build = versInfo.FilePrivatePart;

                string fileNameNoExt = Path.GetFileNameWithoutExtension(fileName);
                string last = fileNameNoExt.Split('_').Last();
                string before = fileNameNoExt.Substring(0,fileNameNoExt.Length - last.Length -1);

                DateTime mod = File.GetLastWriteTime(fileName);
                string newFileName = $"{before}_{major}{minor}{patch}_{build}-{mod.ToString("yyMMdd-HHmm")}-ai.exe";
                File.Delete(newFileName);
                File.Copy(fileName, newFileName);
                Console.WriteLine($"Copied {fileName} to {newFileName}");
                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Unable to copy/rename {args[0]}!");
                Console.WriteLine(e.Message);
                return 1;
            }
        }
    }
}
