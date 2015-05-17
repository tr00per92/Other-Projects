namespace AbcSlider.DemoApp.Converters
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    public class StyleConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var targetElement = values[0] as FrameworkElement;
            var styleName = values[1] as string;
            if (targetElement == null ||
                string.IsNullOrWhiteSpace(styleName) || 
                styleName == "Default")
            {
                return null;
            }

            var newStyle = (Style)targetElement.TryFindResource(styleName);

            return newStyle;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
