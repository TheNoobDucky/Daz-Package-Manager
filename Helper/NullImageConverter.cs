using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Helpers
{
    // https://stackoverflow.com/questions/5399601/imagesourceconverter-error-for-source-null
    public class NullImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str && str != null)
            {
                try
                {
                    return new BitmapImage(new Uri(str, UriKind.RelativeOrAbsolute));
                }
                catch (NotSupportedException)
                {
                }
            }
            return DependencyProperty.UnsetValue;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // According to https://msdn.microsoft.com/en-us/library/system.windows.data.ivalueconverter.convertback(v=vs.110).aspx#Anchor_1
            // (kudos Scott Chamberlain), if you do not support a conversion 
            // back you should return a Binding.DoNothing or a 
            // DependencyProperty.UnsetValue
            return Binding.DoNothing;
            // Original code:
            // throw new NotImplementedException();
        }
    }
}
