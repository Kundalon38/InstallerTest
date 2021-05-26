namespace RISA_CustomActionsLib
{
    public partial class CustomActions
    {
        // many consts are public for the sake of unit testing
        // - they really should be private

        #region Property Name consts

        public const string _propAI_APPDIR = "APPDIR";
        public const string _propMSI_ProductName = "ProductName";
        public const string _propRISA_REGISTRY_PRODUCT_NAME = "RISA_REGISTRY_PRODUCT_NAME";
        public const string _propMSI_ProductVersion = "ProductVersion";
        public const string _propMSI_TARGETDIR = "TARGETDIR";
        public const string _propMSI_CustomActionData = "CustomActionData";

        public const string _propRISA_COMPANY_KEY = "RISA_COMPANY_KEY";
        public const string _propRISA_INSTALL_TYPE = "RISA_INSTALL_TYPE";
        public const string _propRISA_IS_ROAMING_PROFILE = "RISA_IS_ROAMING_PROFILE";

        public const string _propRISA_LICENSE_TYPE = "RISA_LICENSE_TYPE";
        public const string _propRISA_PRODUCT_TITLE2_INSTYPE = "RISA_PRODUCT_TITLE2_INSTYPE";
        public const string _propRISA_PRODUCT_VERSION2 = "RISA_PRODUCT_VERSION2";
        public const string _propRISA_PRODUCT_VERSION34 = "RISA_PRODUCT_VERSION34";

        public const string _propRISA_REGION_NAME = "RISA_REGION_NAME";
        public const string _propRISA_UPDATE_DATA_VALUE = "RISA_UPDATE_DATA_VALUE";
        public const string _propRISA_USERFILES = "RISA_USERFILES";

        public const string _propRISA_STATUS_CODE = "RISA_STATUS_CODE";
        public const string _propRISA_STATUS_TEXT = "RISA_STATUS_TEXT";
        public const string _propRISA_CA_DEBUG = "RISA_CA_DEBUG";

        public const string _debug_Trace = "TRACEFILE";

        #region Transfer properties - from One CA to Another

        public const string _propRISA_INSTALLED_PRODUCTS = "RISA_INSTALLED_PRODUCTS";
        public const string _propRISA_SI_PREINSTALL_RESULT = "RISA_SI_PREINSTALL_RESULT";

        #endregion

        #endregion

        #region RISA_STATUS_CODE values

        public const string _sts_EXCP = "RISA_STS_EXCP";
        public const string _sts_OK = "RISA_STS_OK";
        public const string _sts_ERR_INSTALL_OLD_VERSION = "RISA_ERR_INSTALL_OLD_VERSION";
        public const string _sts_ERR_REMOVE_INSTALLED_PRODUCT = "RISA_ERR_REMOVE_INSTALLED_PRODUCT";
        public const string _sts_ERR_PRODUCT_ACTIVE = "RISA_ERR_PRODUCT_ACTIVE";

        public const string _sts_BAD_DEST_DIR = "RISA_BAD_DEST_DIR";
        public const string _sts_BAD_INSTALLTYPE = "RISA_BAD_INSTALLTYPE";
        public const string _sts_BAD_PRODUCTNAME = "RISA_BAD_PRODUCTNAME";
        public const string _sts_BAD_PRODUCTVERSION = "RISA_BAD_PRODUCTVERSION";

        // _propRISA_SI_PREINSTALL_RESULT values must be numeric.ToString()

        public const string _sts_SILENT_OK = "1";
        public const string _sts_SILENT_EXCP = "0";
        public const string _sts_SILENT_ERR_REMOVE_INSTALLED_PRODUCT = "-1";

        public const string _stsText_Success = "Success";

        #endregion

        #region Valid Values

        // generally lists are ordered most popular to least (find most popular first)

        #region Installation Types

        public const string _insTypeDemo = "Demo";
        public const string _insTypeStandalone = "Standalone";

        private static readonly string[] _insTypeList = new[]
        {
            _insTypeStandalone,  _insTypeDemo
        };


        #endregion

        #region Product Names

        private const string _productRISA2D = "RISA-2D";
        private const string _productRISA3D = "RISA-3D";
        private const string _productRISAFD = "RISAFoundation";
        private const string _productRISAFL = "RISAFloor";
        private const string _productRISACN = "RISAConnection";

        // abbrevs are only for RISA products - see InstallAware script initVariables
        private const string _updAbbrevRISA2D = "2D";
        private const string _updAbbrevRISA3D = "3D";
        private const string _updAbbrevRISAFD = "FD";
        private const string _updAbbrevRISAFL = "RF";
        private const string _updAbbrevRISACN = "RC";


        private static readonly string[] _productNameList = new[]
        {
            _productRISA3D,  _productRISAFD, _productRISAFL,
            _productRISACN,  _productRISA2D
        };
        // the products in both of these arrays must correspond with each other exactly
        private static readonly string[] _updateProductAbbrevs = new[]
        {
            _updAbbrevRISA3D, _updAbbrevRISAFD, _updAbbrevRISAFL,
            _updAbbrevRISACN, _updAbbrevRISA2D
        };

        #endregion

        #region Regions

        private const string _regionUSA = "UNITED_STATES";
        private const string _regionCANADA = "CANADA";
        private const string _regionUK = "BRITAIN";
        private const string _regionEUROPE = "EUROPE";
        private const string _regionINDIA = "INDIA";
        private const string _regionOZ = "AUSTRALIA";
        private const string _regionNZ = "NEW_ZEALAND";
        private const string _regionMEXICO = "MEXICO";
        private const string _regionSAUDI = "SAUDI_ARABIA";

        // order is critical, silent file syntax requires user to provide an index
        private static readonly string[] _regionNameList = new[]
        {
            _regionUSA, _regionCANADA, _regionUK, _regionEUROPE, _regionINDIA,
            _regionOZ, _regionNZ, _regionMEXICO, _regionSAUDI
        };

        private const string _defRegionName = _regionUSA;

        #endregion

        #endregion

        #region Misc consts

        private const string _licenseTypeSubKeyName = "License Type";
        private const string _defLicenseType = "Cloud";
        public const string _boolTrue = "True";
        public const string _boolFalse = "False";

        #endregion
    }
}
