﻿using System;
using System.Globalization;
using System.Windows.Data;

namespace FindRemInstalledProductsWPF.Helpers
{
    public class ConvertToRbChecked : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.Equals(parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.Equals(true) ? parameter : Binding.DoNothing;
        }
    }
}
