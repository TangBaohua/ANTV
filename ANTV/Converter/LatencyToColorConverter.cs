using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ANTV.Converter
{

    public class LatencyToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int latency)
            {
                // 根据延迟值返回对应颜色
                if (latency < 2000) return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#008000"));
                if (latency < 3000) return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFD700"));
                if (latency < 4000) return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF8C00"));
                if (latency < 5001) return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#B22222"));
            }
            return new SolidColorBrush(Colors.Transparent);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
