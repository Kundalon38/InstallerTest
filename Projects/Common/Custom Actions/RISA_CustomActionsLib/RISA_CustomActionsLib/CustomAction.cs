using Microsoft.Deployment.WindowsInstaller;
using System;
using System.IO;

namespace RISA_CustomActionsLib
{
    // note that this this lib is built against .NET 4.5
    // - very low bar, most / all customer machines wil have this,
    //   eliminating the need to provision the customer machine for this lib
    //
    public class CustomActions
    {
        [CustomAction]
        public static ActionResult DetectRoaming(Session session)
        {
            const string outputPropertyName = "USERFILES_RISA";
            const string outputDirIfRoaming = @"C:\RISA";
            //
            // typical myDocsPath:      C:\Users\<username>\Documents
            // typical userProfilePath: C:\Users\<username>
            //
            var myDocsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var userProfilePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            var isRoaming = myDocsPath.StartsWith(@"\\");       // roaming if a unc path
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

            session[outputPropertyName] = isRoaming
                ? outputDirIfRoaming
                : Path.Combine(myDocsPath, "RISA");

            return ActionResult.Success;
        }
    }
}
