using System;
using System.Globalization;
using System.Windows.Data;

namespace ANTV.Converter
{
    public class LatencyToWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int latency)
            {
                // 根据延迟值返回对应宽度
                return  300 * ((decimal)latency / 5000);
               // return latency * 2; // 比如每单位延迟宽度设置为 2px
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}