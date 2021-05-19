using System;
using System.Diagnostics;
using RISA_CustomActionsLib.Extensions;

namespace RISA_CustomActionsLib.Models
{
    public class BootstrapperData
    {
        public BootstrapperData(string cmdLine)
        {
            if (cmdLine == null) return;

            CmdLine = cmdLine;
            var cmdLineUC = cmdLine.ToUpper();
            if (!cmdLineUC.Contains(@"/QN") && !cmdLineUC.Contains(@"/QUIET")) return; // not silent
            IsSilent = true;
        }

        public string CmdLine { get; }

        public bool IsSilent { get; }

        public string ProductName { get; private set; }
        public string ProductVersionStr { get; private set; }

        public void LoadFileVersionInfo(FileVersionInfo fvi)
        {
            ProductName = fvi.ProductName;
            ProductVersionStr = fvi.ProductVersion;
        }

        public override string ToString()
        {
            var cr = Environment.NewLine;
            return
                $"CmdLine={CmdLine.ToDetailStr()}{cr}IsSilent={IsSilent}{cr}ProductName={ProductName.ToDetailStr()}{cr}ProductVersion={ProductVersionStr}";
        }

    }
}
