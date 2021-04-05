using Caliburn.Micro;
using FindRemInstalledProductsWPF.Helpers;
using System;
using System.Linq;
using System.Windows;
using RISA_CustomActionsLib;
using RISA_CustomActionsLib.Models;

namespace FindRemInstalledProductsWPF.ViewModels
{
    public enum eInsType {Demo, Standalone}
    public class MainViewModel : PropertyChangedBase
    {
        public MainViewModel()
        {
            Messages = new BindableCollection<LogMessage>();
            ProductName = "RISA-3D";
            _sessDTO = new SessionDTO(display)
                {[CustomActions_StopStartService._propRISA_INSTALLED_PRODUCTS] = string.Empty};
        }
        private SessionDTO _sessDTO;


        #region UI

        public string ProductName
        {
            get => _productName;
            set
            {
                if (_productName == value) return;
                _productName = value;
                NotifyOfPropertyChange(() => ProductName);
                makeDisplayName();
            }
        }
        private string _productName;

        public eInsType InstallType
        {
            get => _insType;
            set
            {
                if (_insType == value) return;
                _insType = value;
                NotifyOfPropertyChange(() => InstallType);
                makeDisplayName();
            }
        }
        private eInsType _insType;
        public static eInsType Demo => eInsType.Demo;
        public static eInsType Standalone => eInsType.Standalone;

        public string ProductVersion
        {
            get => _productVersion;
            set
            {
                if (_productVersion == value) return;
                _productVersion = value;
                NotifyOfPropertyChange(() => ProductVersion);
                makeDisplayName();
            }
        }
        private string _productVersion;

        public void ClearVersion()
        {
            ProductVersion = string.Empty;
        }

        private void makeDisplayName()
        {
            DisplayName = string.Empty;
            if (string.IsNullOrEmpty(ProductName)) return;
            if (string.IsNullOrEmpty(ProductVersion)) return;
            if (ProductVersion.VersionPartsCount() < 3) return;
            //
            var insTypeStr = InstallType == eInsType.Demo ? "Demo" : string.Empty;
            DisplayName = $"{ProductName} {ProductVersion.ToVersion2()} {insTypeStr}";
        }

        public string DisplayName
        {
            get => _displayName;
            set
            {
                if (_displayName == value) return;
                _displayName = value;
                NotifyOfPropertyChange(() => DisplayName);
            }
        }
        private string _displayName;

        public void FindProducts()
        {
            _sessDTO[CustomActions_StopStartService._propMSI_ProductName] = ProductName;
            _sessDTO[CustomActions_StopStartService._propMSI_ProductVersion] = ProductVersion;
            _sessDTO[CustomActions_StopStartService._propMSI_TARGETDIR] = string.Empty;
            _sessDTO[CustomActions_StopStartService._propRISA_INSTALL_TYPE] = InstallType.ToString();
            _sessDTO[CustomActions_StopStartService._propRISA_PRODUCT_TITLE2_INSTYPE] = DisplayName;
            _sessDTO[CustomActions_StopStartService._propRISA_INSTALLED_PRODUCTS] = string.Empty;
            _sessDTO[CustomActions_StopStartService._propRISA_STATUS_CODE] = string.Empty;
            _sessDTO[CustomActions_StopStartService._propRISA_STATUS_TEXT] = string.Empty;
            var result = CustomActions_StopStartService.serializeMatchingInstalledProducts(_sessDTO);
            display($"serializeMatchingInstalledProducts returns {result}");
            display(_sessDTO.ToString());
        }

        public string TargetDir
        {
            get => _targetDir;
            set
            {
                if (_targetDir == value) return;
                _targetDir = value;
                NotifyOfPropertyChange(() => TargetDir);
                NotifyOfPropertyChange(() => CanRemoveProducts);
            }
        }
        private string _targetDir;

        public void RemoveProducts()
        {
            _sessDTO[CustomActions_StopStartService._propMSI_TARGETDIR] = TargetDir;
            CustomActions_StopStartService.removeInstalledProducts(_sessDTO);
        }

        public bool CanRemoveProducts => !string.IsNullOrEmpty(TargetDir);
        #endregion

        #region Misc

        private BindableCollection<LogMessage> _messages;
        public BindableCollection<LogMessage> Messages
        {
            get => _messages;
            set
            {
                if (_messages == value) return;
                _messages = value;
                NotifyOfPropertyChange(() => Messages);
            }
        }

        private void display(string msg)
        {
            Messages.Add(new LogMessage(msg));
        }

        private void displayExcp(Exception ex, string loc = null)
        {
            var logMsg = new LogMessage(eLogMessageType.Error, ex.Message);
            Messages.Add(logMsg);
        }

        private void displayErr(string msg)
        {
            Messages.Add(new LogMessage(eLogMessageType.Error, msg));
        }

        public void Clear()
        {
            Messages.Clear();
        }

        public void Copy()
        {
            // Reverse() puts oldest first, for printing/reading like a book
            Clipboard.SetText(string.Join(Environment.NewLine, Messages.Reverse().Select(x => x.ToString())));
        }

        #endregion
    }
}
