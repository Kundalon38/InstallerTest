using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RISA_CustomActionsLib.Models;

namespace RISA_CustomActionsLib.Test.LoudCAs_Tests
{
    [TestClass]
    public class InitProperties_Tests
    {
        [TestInitialize]
        public void TestInit()
        {
            _sessDTO = new SessionDTO(Console.WriteLine)
            {
                // init property names - clone of initSessionDTO()
                // props set by installer

                [CustomActions._propMSI_ProductName] = string.Empty,
                [CustomActions._propMSI_ProductVersion] = string.Empty,
                [CustomActions._propRISA_COMPANY_KEY] = "RISA Technologies",
                [CustomActions._propRISA_INSTALL_TYPE] = string.Empty,

                // props set by Custom Action
                [CustomActions._propRISA_LICENSE_TYPE] = string.Empty,
                [CustomActions._propRISA_PRODUCT_TITLE2_INSTYPE] = string.Empty,
                [CustomActions._propRISA_PRODUCT_VERSION2] = string.Empty,
                [CustomActions._propRISA_PRODUCT_VERSION34] = string.Empty,
                [CustomActions._propRISA_STATUS_CODE] = string.Empty,
                [CustomActions._propRISA_STATUS_TEXT] = string.Empty,
                [CustomActions._propRISA_UPDATE_DATA_VALUE] = string.Empty,
                [CustomActions._propRISA_USERFILES] = string.Empty
            };
        }
        private SessionDTO _sessDTO;
        private const string _risa3D = "RISA-3D";
        private const string _risaFloor = "RISAFloor";

        #region ProductName tests

        [TestMethod]
        [TestCategory("validProductName")]
        public void ProductName_InValid()
        {
            _sessDTO[CustomActions._propMSI_ProductName] = "MyProduct";
            _sessDTO[CustomActions._propMSI_ProductVersion] = "1.0.0";
            _sessDTO[CustomActions._propRISA_INSTALL_TYPE] = "Standalone";

            var result = CustomActions.validProductName(_sessDTO);
            expecting(returnCode(CustomActions._sts_BAD_PRODUCTNAME));
            Assert.IsFalse(result);
        }

        [TestMethod]
        [TestCategory("validProductName")]
        public void ProductName_Valid()
        {
            _sessDTO[CustomActions._propMSI_ProductName] = _risa3D;
            _sessDTO[CustomActions._propMSI_ProductVersion] = "1.0.0";
            _sessDTO[CustomActions._propRISA_INSTALL_TYPE] = "Standalone";

            var result = CustomActions.validProductName(_sessDTO);
            Assert.IsTrue(result);
        }

        #endregion

        #region Install Type tests

        [TestMethod]
        [TestCategory("validInstallType")]
        public void InstallType_InValid()
        {
            _sessDTO[CustomActions._propMSI_ProductName] = "MyProduct";
            _sessDTO[CustomActions._propMSI_ProductVersion] = "1.0.0";
            _sessDTO[CustomActions._propRISA_INSTALL_TYPE] = "";

            var result = CustomActions.validInstallType(_sessDTO);
            expecting(returnCode(CustomActions._sts_BAD_INSTALLTYPE));
            Assert.IsFalse(result);
        }

        [TestMethod]
        [TestCategory("validInstallType")]
        public void InstallType_Standalone_Valid()
        {
            _sessDTO[CustomActions._propMSI_ProductName] = _risa3D;
            _sessDTO[CustomActions._propMSI_ProductVersion] = "1.0.0";
            _sessDTO[CustomActions._propRISA_INSTALL_TYPE] = "Standalone";

            var result = CustomActions.validInstallType(_sessDTO);
            Assert.IsTrue(result);
        }

        [TestMethod]
        [TestCategory("validInstallType")]
        public void InstallType_Demo_Valid()
        {
            _sessDTO[CustomActions._propMSI_ProductName] = _risa3D;
            _sessDTO[CustomActions._propMSI_ProductVersion] = "1.0.0";
            _sessDTO[CustomActions._propRISA_INSTALL_TYPE] = "Demo";

            var result = CustomActions.validInstallType(_sessDTO);
            Assert.IsTrue(result);
        }

        #endregion

        #region ProcessProductVersion

        [TestMethod]
        [TestCategory("processProductVersion")]
        public void ProductVersion_2_InValid()
        {
            _sessDTO[CustomActions._propMSI_ProductName] = _risa3D;
            _sessDTO[CustomActions._propMSI_ProductVersion] = "1.0";
            _sessDTO[CustomActions._propRISA_INSTALL_TYPE] = "Standalone";

            var result = CustomActions.processProductVersion(_sessDTO);
            expecting(returnCode(CustomActions._sts_BAD_PRODUCTVERSION));
            Assert.IsFalse(result);
        }
        [TestMethod]
        [TestCategory("processProductVersion")]
        public void ProductVersion_Omitted_InValid()
        {
            _sessDTO[CustomActions._propMSI_ProductName] = _risa3D;
            _sessDTO[CustomActions._propMSI_ProductVersion] = string.Empty;
            _sessDTO[CustomActions._propRISA_INSTALL_TYPE] = "Standalone";

            var result = CustomActions.processProductVersion(_sessDTO);
            expecting(returnCode(CustomActions._sts_BAD_PRODUCTVERSION));
            Assert.IsFalse(result);
        }
        [TestMethod]
        [TestCategory("processProductVersion")]
        public void ProductVersion_3_Valid()
        {
            _sessDTO[CustomActions._propMSI_ProductName] = _risa3D;
            _sessDTO[CustomActions._propMSI_ProductVersion] = "1.0.0";
            _sessDTO[CustomActions._propRISA_INSTALL_TYPE] = "Standalone";

            var result = CustomActions.processProductVersion(_sessDTO);
            Assert.IsTrue(result);
        }
        [TestMethod]
        [TestCategory("processProductVersion")]
        public void ProductVersion_4_Valid()
        {
            _sessDTO[CustomActions._propMSI_ProductName] = _risa3D;
            _sessDTO[CustomActions._propMSI_ProductVersion] = "1.0.0.0";
            _sessDTO[CustomActions._propRISA_INSTALL_TYPE] = "Standalone";

            var result = CustomActions.processProductVersion(_sessDTO);
            Assert.IsTrue(result);
        }

        #endregion

        #region Assign Version based props

        [TestMethod]
        [TestCategory("assignVersionBasedProperties")]
        public void ProductVersion_3_Demo_Assign()
        {
            _sessDTO[CustomActions._propMSI_ProductName] = _risa3D;
            _sessDTO[CustomActions._propMSI_ProductVersion] = "1.2.3";
            _sessDTO[CustomActions._propRISA_INSTALL_TYPE] = "Demo";

            var result = CustomActions.processProductVersion(_sessDTO);
            expecting(result);
            CustomActions.assignVersionBasedProperties(_sessDTO);
            expecting(_sessDTO[CustomActions._propRISA_PRODUCT_VERSION2] == "1.2");
            expecting(_sessDTO[CustomActions._propRISA_PRODUCT_TITLE2_INSTYPE] == $"{_risa3D} 1.2 Demo");
            expecting(_sessDTO[CustomActions._propRISA_PRODUCT_VERSION34] == "3.0");
            Assert.IsTrue(result);
        }
        [TestMethod]
        [TestCategory("assignVersionBasedProperties")]
        public void ProductVersion_4_Demo_Assign()
        {
            _sessDTO[CustomActions._propMSI_ProductName] = _risa3D;
            _sessDTO[CustomActions._propMSI_ProductVersion] = "1.2.3.4";
            _sessDTO[CustomActions._propRISA_INSTALL_TYPE] = "Demo";

            var result = CustomActions.processProductVersion(_sessDTO);
            expecting(result);
            CustomActions.assignVersionBasedProperties(_sessDTO);
            expecting(_sessDTO[CustomActions._propRISA_PRODUCT_VERSION2] == "1.2");
            expecting(_sessDTO[CustomActions._propRISA_PRODUCT_TITLE2_INSTYPE] == $"{_risa3D} 1.2 Demo");
            expecting(_sessDTO[CustomActions._propRISA_PRODUCT_VERSION34] == "3.4");
            Assert.IsTrue(result);
        }

        [TestMethod]
        [TestCategory("assignVersionBasedProperties")]
        public void ProductVersion_3_Standalone_Assign()
        {
            _sessDTO[CustomActions._propMSI_ProductName] = _risa3D;
            _sessDTO[CustomActions._propMSI_ProductVersion] = "1.2.3";
            _sessDTO[CustomActions._propRISA_INSTALL_TYPE] = "Standalone";

            var result = CustomActions.processProductVersion(_sessDTO);
            expecting(result);
            CustomActions.assignVersionBasedProperties(_sessDTO);
            expecting(_sessDTO[CustomActions._propRISA_PRODUCT_VERSION2] == "1.2");
            expecting(_sessDTO[CustomActions._propRISA_PRODUCT_TITLE2_INSTYPE] == $"{_risa3D} 1.2");
            expecting(_sessDTO[CustomActions._propRISA_PRODUCT_VERSION34] == "3.0");
            Assert.IsTrue(result);
        }
        [TestMethod]
        [TestCategory("assignVersionBasedProperties")]
        public void ProductVersion_4_Standalone_Assign()
        {
            _sessDTO[CustomActions._propMSI_ProductName] = _risa3D;
            _sessDTO[CustomActions._propMSI_ProductVersion] = "1.2.3.4";
            _sessDTO[CustomActions._propRISA_INSTALL_TYPE] = "Standalone";

            var result = CustomActions.processProductVersion(_sessDTO);
            expecting(result);
            CustomActions.assignVersionBasedProperties(_sessDTO);
            expecting(_sessDTO[CustomActions._propRISA_PRODUCT_VERSION2] == "1.2");
            expecting(_sessDTO[CustomActions._propRISA_PRODUCT_TITLE2_INSTYPE] == $"{_risa3D} 1.2");
            expecting(_sessDTO[CustomActions._propRISA_PRODUCT_VERSION34] == "3.4");
            Assert.IsTrue(result);
        }

        #endregion

        #region Assign Remaining Identity props

        [TestMethod]
        [TestCategory("assignRemainingIdentityBasedProperties")]
        public void R3D_Demo_Assign()
        {
            _sessDTO[CustomActions._propMSI_ProductName] = _risa3D;
            _sessDTO[CustomActions._propMSI_ProductVersion] = "1.2.3.4";
            _sessDTO[CustomActions._propRISA_INSTALL_TYPE] = "Demo";

            CustomActions.assignRemainingIdentityBasedProperties(_sessDTO);
            Assert.IsTrue(_sessDTO[CustomActions._propRISA_UPDATE_DATA_VALUE] == "UpdateData3D_Demo");
        }

        [TestMethod]
        [TestCategory("assignRemainingIdentityBasedProperties")]
        public void R3D_Standalone_Assign()
        {
            _sessDTO[CustomActions._propMSI_ProductName] = _risa3D;
            _sessDTO[CustomActions._propMSI_ProductVersion] = "1.2.3";
            _sessDTO[CustomActions._propRISA_INSTALL_TYPE] = "Standalone";

            CustomActions.assignRemainingIdentityBasedProperties(_sessDTO);
            Assert.IsTrue(_sessDTO[CustomActions._propRISA_UPDATE_DATA_VALUE] == "UpdateData3D_SA");
        }

        [TestMethod]
        [TestCategory("assignRemainingIdentityBasedProperties")]
        public void Floor_Demo_Assign()
        {
            _sessDTO[CustomActions._propMSI_ProductName] = _risaFloor;
            _sessDTO[CustomActions._propMSI_ProductVersion] = "1.2.3.4";
            _sessDTO[CustomActions._propRISA_INSTALL_TYPE] = "Demo";

            CustomActions.assignRemainingIdentityBasedProperties(_sessDTO);
            Assert.IsTrue(_sessDTO[CustomActions._propRISA_UPDATE_DATA_VALUE] == "UpdateDataRF_Demo");
        }

        [TestMethod]
        [TestCategory("assignRemainingIdentityBasedProperties")]
        public void Floor_Standalone_Assign()
        {
            _sessDTO[CustomActions._propMSI_ProductName] = _risaFloor;
            _sessDTO[CustomActions._propMSI_ProductVersion] = "1.2.3";
            _sessDTO[CustomActions._propRISA_INSTALL_TYPE] = "Standalone";

            CustomActions.assignRemainingIdentityBasedProperties(_sessDTO);
            Assert.IsTrue(_sessDTO[CustomActions._propRISA_UPDATE_DATA_VALUE] == "UpdateDataRF_SA");
        }

        #endregion

        #region Document Path (roaming profile)

        [TestMethod]
        [TestCategory("assignDocumentPath")]
        public void DocPath_Demo_NoRoam()
        {
            _sessDTO[CustomActions._propMSI_ProductName] = _risa3D;
            _sessDTO[CustomActions._propMSI_ProductVersion] = "1.2.3.4";
            _sessDTO[CustomActions._propRISA_INSTALL_TYPE] = "Demo";

            CustomActions.assignDocumentPath(_sessDTO, false);

            var myDocsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var expectPath = Path.Combine(myDocsPath, "RISADemo");

            expecting(_sessDTO[CustomActions._propRISA_IS_ROAMING_PROFILE] == CustomActions._boolFalse);
            expecting(_sessDTO[CustomActions._propRISA_USERFILES] == expectPath);
            Assert.IsTrue(true);
        }

        [TestMethod]
        [TestCategory("assignDocumentPath")]
        public void DocPath_Standalone_NoRoam()
        {
            _sessDTO[CustomActions._propMSI_ProductName] = _risa3D;
            _sessDTO[CustomActions._propMSI_ProductVersion] = "1.2.3.4";
            _sessDTO[CustomActions._propRISA_INSTALL_TYPE] = "Standalone";

            CustomActions.assignDocumentPath(_sessDTO, false);

            var myDocsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var expectPath = Path.Combine(myDocsPath, "RISA");

            expecting(_sessDTO[CustomActions._propRISA_IS_ROAMING_PROFILE] == CustomActions._boolFalse);
            expecting(_sessDTO[CustomActions._propRISA_USERFILES] == expectPath);
            Assert.IsTrue(true);
        }
        [TestMethod]
        [TestCategory("assignDocumentPath")]
        public void DocPath_Demo_Roam()
        {
            _sessDTO[CustomActions._propMSI_ProductName] = _risa3D;
            _sessDTO[CustomActions._propMSI_ProductVersion] = "1.2.3.4";
            _sessDTO[CustomActions._propRISA_INSTALL_TYPE] = "Demo";

            CustomActions.assignDocumentPath(_sessDTO, true);

            var expectPath = @"C:\RISADemo";

            expecting(_sessDTO[CustomActions._propRISA_IS_ROAMING_PROFILE] == CustomActions._boolTrue);
            expecting(_sessDTO[CustomActions._propAI_APPDIR] == expectPath);
            expecting(_sessDTO[CustomActions._propRISA_USERFILES] == expectPath);
            Assert.IsTrue(true);
        }

        [TestMethod]
        [TestCategory("assignDocumentPath")]
        public void DocPath_Standalone_Roam()
        {
            _sessDTO[CustomActions._propMSI_ProductName] = _risa3D;
            _sessDTO[CustomActions._propMSI_ProductVersion] = "1.2.3.4";
            _sessDTO[CustomActions._propRISA_INSTALL_TYPE] = "Standalone";

            CustomActions.assignDocumentPath(_sessDTO, true);

            var expectPath = @"C:\RISA";

            expecting(_sessDTO[CustomActions._propRISA_IS_ROAMING_PROFILE] == CustomActions._boolTrue);
            expecting(_sessDTO[CustomActions._propAI_APPDIR] == expectPath);
            expecting(_sessDTO[CustomActions._propRISA_USERFILES] == expectPath);
            Assert.IsTrue(true);
        }

        #region OneDrive

        [TestMethod]
        [TestCategory("pathContainsOneDrive")]
        public void Typical_OneDrive_Path()
        {
            const string testPath = @"C:\Users\Bob\OneDrive";
            Assert.IsTrue(CustomActions.pathContainsOneDrive(testPath));
        }
        [TestMethod]
        [TestCategory("pathContainsOneDrive")]
        public void Longer_OneDrive_Path()
        {
            const string testPath = @"C:\Users\Bob\OneDrive\Documents";
            Assert.IsTrue(CustomActions.pathContainsOneDrive(testPath));
        }
        [TestMethod]
        [TestCategory("pathContainsOneDrive")]
        public void Not_OneDrive_Path()
        {
            const string testPath = @"C:\Users\Bob\";
            Assert.IsFalse(CustomActions.pathContainsOneDrive(testPath));
        }

        #endregion

        #endregion

        #region Validate Install Directory (roaming profile)

        [TestMethod] [TestCategory("validateInstallDirectory")]
        public void Roam_InsDir_OK()
        {
            var expectPath = @"C:\SomeDir";
            _sessDTO[CustomActions._propMSI_ProductName] = _risa3D;
            _sessDTO[CustomActions._propMSI_ProductVersion] = "1.2.3.4";
            _sessDTO[CustomActions._propRISA_INSTALL_TYPE] = "Standalone";
            _sessDTO[CustomActions._propAI_APPDIR] = expectPath;

            CustomActions.validateInstallDirectory(_sessDTO, true);

            expecting(_sessDTO[CustomActions._propAI_APPDIR] == expectPath);
            expecting(returnCode(CustomActions._sts_OK));
            Assert.IsTrue(true);
        }

        [TestMethod]
        [TestCategory("validateInstallDirectory")]
        public void Roam_InsDir_Bad()
        {
            var expectPath = @"C:\RISA";
            _sessDTO[CustomActions._propMSI_ProductName] = _risa3D;
            _sessDTO[CustomActions._propMSI_ProductVersion] = "1.2.3.4";
            _sessDTO[CustomActions._propRISA_INSTALL_TYPE] = "Standalone";
            _sessDTO[CustomActions._propAI_APPDIR] = @"C:\Program Files\RISA";

            CustomActions.validateInstallDirectory(_sessDTO, true);

            expecting(_sessDTO[CustomActions._propAI_APPDIR] == expectPath);
            expecting(returnCode(CustomActions._sts_BAD_DEST_DIR));
            Assert.IsTrue(true);
        }

        [TestMethod]
        [TestCategory("validateInstallDirectory")]
        public void NotRoam_InsDir()
        {
            _sessDTO[CustomActions._propMSI_ProductName] = _risa3D;
            _sessDTO[CustomActions._propMSI_ProductVersion] = "1.2.3.4";
            _sessDTO[CustomActions._propRISA_INSTALL_TYPE] = "Standalone";
            _sessDTO[CustomActions._propAI_APPDIR] = @"C:\Program Files\RISA";

            CustomActions.validateInstallDirectory(_sessDTO, false);

            //expecting(_sessDTO[CustomActions._propAI_APPDIR] == expectPath);
            expecting(returnCode(CustomActions._sts_OK));
            Assert.IsTrue(true);
        }



        #endregion

        #region Default License Type

        [TestMethod]
        [TestCategory("assignDefaultLicenseType")]
        public void Debug_LicenseType()
        {
            // this "test" requires Admin privs and requires your computer
            //   to have installed 3D at some time (it need not be currently installed)
            // use the Debugger to step through assignDefaultLicenseType()
            //
            // if neither of these conditions aren met, the "Test" will still pass
            //
            _sessDTO[CustomActions._propMSI_ProductName] = _risa3D;
            _sessDTO[CustomActions._propMSI_ProductVersion] = "1.2.3.4";
            _sessDTO[CustomActions._propRISA_INSTALL_TYPE] = "Standalone";
            _sessDTO[CustomActions._propRISA_REGISTRY_PRODUCT_NAME] = _risa3D;

            CustomActions.assignDefaultLicenseType(_sessDTO);
            Console.WriteLine(_sessDTO[CustomActions._propRISA_LICENSE_TYPE]);
            Assert.IsTrue(true);
        }

        #endregion

        #region From the top

        [TestMethod]
        [TestCategory("initProperties")]
        public void InitProperties_Success()
        {
            _sessDTO[CustomActions._propMSI_ProductName] = _risa3D;
            // make sure ProductVersion is beyond any possible real version that could actually be installed
            _sessDTO[CustomActions._propMSI_ProductVersion] = "400.2.3.4";
            _sessDTO[CustomActions._propRISA_INSTALL_TYPE] = "Standalone";
            _sessDTO[CustomActions._propRISA_REGISTRY_PRODUCT_NAME] = _risa3D;

            CustomActions.initProperties(_sessDTO);
            Console.WriteLine(_sessDTO.ToString());
            Assert.IsTrue(_sessDTO[CustomActions._propRISA_STATUS_CODE] == CustomActions._sts_OK);
        }

        [TestMethod]
        [TestCategory("initProperties")]
        public void InitProperties_Fail()
        {
            _sessDTO[CustomActions._propMSI_ProductName] = _risa3D;
            _sessDTO[CustomActions._propMSI_ProductVersion] = "1.2.3.4";
            _sessDTO[CustomActions._propRISA_INSTALL_TYPE] = "MyProduct";
            _sessDTO[CustomActions._propRISA_REGISTRY_PRODUCT_NAME] = _risa3D;

            CustomActions.initProperties(_sessDTO);
            Console.WriteLine(_sessDTO.ToString());
            Assert.IsFalse(_sessDTO[CustomActions._propRISA_STATUS_CODE] == CustomActions._sts_OK);
        }

        #endregion

        #region Helpers

        private void expecting(bool condition)
        {
            if (!condition) Assert.IsTrue(false);
        }
        private bool returnCode(string returnCodeValue)
        {
            return (_sessDTO[CustomActions._propRISA_STATUS_CODE] == returnCodeValue);
        }

        #endregion
    }
}
