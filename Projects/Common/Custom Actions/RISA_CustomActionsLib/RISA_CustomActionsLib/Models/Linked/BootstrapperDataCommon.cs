using System.Collections.Generic;

namespace RISA_CustomActionsLib.Models.Linked
{
    public class BootstrapperDataCommon
    {
        public const string _propInsDir = "SIDIR";
        public const string _propPgmGrp = "SIGRP";
        public const string _propRegion = "SIRGN";
        public const string _propUpdate = "SIUPD";
        public const string _propLicType = "SILTY";
        public const string _propLogFile = "SILOG";
        public const string _propIniFile = "SINIF";

        protected readonly string[] _supportedSiPropNames = { _propInsDir, _propPgmGrp, _propRegion, _propUpdate, _propLicType, _propLogFile, _propIniFile };

        // consts below are 
        // used only by BootstrapperIniFile, but declared in this parent class because:
        // - the elements in _supportedSiPropNames and _supportedIniPropNames are tied together
        //
        protected const string _iniPropInsDir = "InstallationDirectory";
        protected const string _iniPropPgmGrp = "ProgramGroup";
        protected const string _iniPropRegion = "Region";
        protected const string _iniPropUpdate = "AutoCheckForUpdates";
        protected const string _iniPropLicType = "LicenseType";

        protected readonly string[] _supportedIniPropNames = { _iniPropInsDir, _iniPropPgmGrp, _iniPropRegion, _iniPropUpdate, _iniPropLicType };


        protected const string _allValidRegions = "012345678";

        protected const string _ltCloud = "Subscription";
        protected const string _ltNetwork = "Network";
        protected const string _ltKey = "Key";
        protected const string _ansYes = "Yes";
        protected const string _ansNo = "No";
        public CmdLineProperties CmdLineProperties { get; protected set; } = new CmdLineProperties();

        public List<SiError> ErrorList { get; set; } = new List<SiError>();

        protected void addErr(string text, bool isFatal = true)
        {
            // a little shorter^ than:
            ErrorList.Add(new SiError(text, isFatal));
        }
    }
}
