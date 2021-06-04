using System;
using System.IO;
using System.Linq;

namespace RISA_CustomActionsLib.Models.Linked
{
    public class BootstrapperIniFile : BootstrapperDataCommon
    {
        private enum eIniState
        {
            FindSection,
            InSection,
            ParseFinished
        }


        public BootstrapperIniFile(string iniFn, string productName)
        {
            StreamReader sr;
            string op = null;
            try
            {
                op = "find";
                if (!File.Exists(iniFn)) throw new ApplicationException();

                op = "open";
                sr = new StreamReader(iniFn);

            }
            catch (Exception e)
            {
                addErr($"Can't {op}: {iniFn}");
                return;
            }
            //
            // sample ini file snippet:
            //
            //
            // ;	Subscription - Subscription / cloud based licensing
            // ;	Key - Perpetual licensing with a stand-alone hardware key
            // ;	Network - Perpetual licensing with a keyed network server
            // ;
            // [RISA-3D]
            // InstallationDirectory=
            // ProgramGroup=RISA
            // Region=0
            // AutoCheckForUpdates=Yes
            // LicenseType=Subscription
            // 
            // [RISAFloor]
            // InstallationDirectory=

            var state = eIniState.FindSection;
            var productSectionFound = false;
            string buf;

            try
            {
                while ((buf = sr.ReadLine()) != null && state != eIniState.ParseFinished)
                {
                    buf = buf.Trim();
                    if (buf.Length == 0) continue;
                    switch (state)
                    {
                        case eIniState.FindSection:
                            if (!buf.StartsWith("[")) break;
                            if (!buf.Substring(1).StartsWith(productName)) break;
                            state = eIniState.InSection;
                            productSectionFound = true;
                            break;

                        case eIniState.InSection:
                            if (buf.StartsWith("["))
                            {
                                state = eIniState.ParseFinished;
                                break;
                            }

                            var tokensRaw = buf.Split('=');
                            var tokens = tokensRaw.Where(x => x.Trim().Length > 0).ToList();
                            var tokenNdx = Array.FindIndex(_supportedIniPropNames, x => x == tokens[0]);
                            if (tokenNdx < 0)
                            {
                                addErr($"Invalid keyword {tokens[0]} in: {iniFn}");
                                state = eIniState.ParseFinished;
                                break;
                            }

                            if (tokens.Count == 1) break; // no value given for property
                            // map from ini file keyword to Si cmd line property name
                            CmdLineProperties.Add(new CmdLineProperty(_supportedSiPropNames[tokenNdx],
                                                                                _supportedIniPropNames[tokenNdx], tokens[1]));
                            break;
                    }
                }

                if (!productSectionFound) addErr($"[{productName}] sectopm not found in {iniFn}");
            }
            catch (Exception e)
            {
                addErr($"Error parsing {iniFn}, {e.Message}");
            }
            finally
            {
                sr.Close();
            }
        }
    }
}
