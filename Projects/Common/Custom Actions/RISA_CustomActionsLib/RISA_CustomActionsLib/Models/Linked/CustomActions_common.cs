using System;
using System.IO;
using System.Linq;

namespace RISA_CustomActionsLib
{
    public partial class CustomActions
    {
        public static string getInstallDir(string userRequestedInsDir, string installType)
        {
            if (userRequestedInsDir == null) // installation dir defaulted, TODO log decision
            {
                return isAproblemProfile()
                    ? altInstallDir(installType)
                    : pgmFilesInsDir(installType);
            }
            // verify that specified ins dir is not a child of C:\Program Files
            //  I believe this is because both ins dir and documents go here (they're unfortunately conflated)
            //   application will fail if it tries to write (documents) to C:\Program Files
            //
            var pgmFilesDir = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            if (userRequestedInsDir.StartsWith(pgmFilesDir, StringComparison.CurrentCultureIgnoreCase))
            {
                return CustomActions.altInstallDir(installType);
            }

            return userRequestedInsDir;
        }

        public static string altInstallDir(string installType)
        {
            // this is about finding the correct drive letter based on where the Windows directory lives
            // nearly everyone will be using C:\
            //
            // typically: return C:\RISA or C:\RISADemo
            //
            const string bash = @"\";
            var drives = DriveInfo.GetDrives();
            var windowsDir = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
            var outDriveIfRoaming = drives.Select(x => x.Name).SingleOrDefault(x => windowsDir.StartsWith(x));
            if (outDriveIfRoaming == null) outDriveIfRoaming = @"C:\";
            else if (!outDriveIfRoaming.EndsWith(bash)) outDriveIfRoaming += bash;

            var folderName = (installType == _insTypeDemo) ? "RISADemo" : "RISA";
            return $"{outDriveIfRoaming}{folderName}";
        }

        public static string pgmFilesInsDir(string installType)
        {
            var subFolderName = (installType == _insTypeDemo) ? "RISADemo" : "RISA";
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), subFolderName);
        }

        public static bool isAproblemProfile(bool? isRoamingValue = null)
        {
            var myDocsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            // typical myDocsPath:      C:\Users\<username>\Documents
            var usingOneDrive = pathContainsOneDrive(myDocsPath);

            // typical userProfilePath: C:\Users\<username>
            var userProfilePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            if (!usingOneDrive) usingOneDrive = pathContainsOneDrive(userProfilePath);

            // roaming if a unc path
            var isRoaming = isRoamingValue.HasValue ? isRoamingValue.Value : myDocsPath.StartsWith(@"\\");

            if (!isRoaming)
            {
                // myDocsPath must stem from userProfilePath, otherwise isRoaming=T; will this comparison always work?
                isRoaming = !myDocsPath.StartsWith(userProfilePath);
            }

            if (!isRoaming)
            {
                // require both paths to exist, otherwise isRoaming=T
                isRoaming = !(Directory.Exists(myDocsPath) && Directory.Exists(userProfilePath));
            }
            return isRoaming || usingOneDrive;
        }

        public static bool pathContainsOneDrive(string path)
        {
            // OneDrive is at:      C:\Users\<username>\OneDrive    users can add subdirs to this
            // but not completely sure of all use cases
            //
            const int bash = 92;
            var pathParts = path.Split((char)bash);
            if (pathParts.Length < 4) return false;     // C: - Users - <username> - OneDrive - maybeMore
            return (pathParts[3] == "OneDrive");
        }
    }
}
