using System;
using Windows.UI.Xaml.Data;

namespace MPKMonitor
{
  public sealed class GPSConverter : IValueConverter
  {    
    public object Convert(object value, Type targetType, object parameter, string language)
    {
      bool check = (bool)value;

      if (check == true)
        return "GPS";

      return " ";
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
      throw new NotImplementedException();
    }
  }

}
