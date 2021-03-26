using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace FindRemInstalledProductsWPF.Helpers
{
    public class ConvertLogMessageTypeToColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var logMsgType = (eLogMessageType)value;
            switch (logMsgType)
            {
                case eLogMessageType.Debug:
                    return _debugColor;

                case eLogMessageType.Error:
                    return _errorColor;

                case eLogMessageType.Normal:
                    return _normalColor;

                case eLogMessageType.Warn:
                    return _warnColor;

            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }


        private static readonly SolidColorBrush _debugColor;
        private static readonly SolidColorBrush _errorColor;
        private static readonly SolidColorBrush _normalColor;
        private static readonly SolidColorBrush _warnColor;

        static ConvertLogMessageTypeToColor()
        {
            _debugColor = (SolidColorBrush)Application.Current.MainWindow.FindResource("LogPanelForeground_Debug");
            _errorColor = (SolidColorBrush)Application.Current.MainWindow.FindResource("LogPanelForeground_Error");
            _normalColor = (SolidColorBrush)Application.Current.MainWindow.FindResource("LogPanelForeground_Normal");
            _warnColor = (SolidColorBrush)Application.Current.MainWindow.FindResource("LogPanelForeground_Warn");
        }
    }
}
