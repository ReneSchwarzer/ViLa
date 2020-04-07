using System;
using System.Collections.Generic;
using System.Text;

namespace MtWb.Model
{
    public class TimeSpanConverter 
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return null;
            }

            var ts = TimeSpan.Parse(value.ToString());
            return string.Format
                (
                    "{0}d {1:D2}h {2:D2}m {3:D2}s",
                    ts.Days,
                    ts.Hours,
                    ts.Minutes,
                    ts.Seconds
                );
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
