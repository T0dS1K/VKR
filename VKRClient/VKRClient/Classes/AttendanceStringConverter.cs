using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Globalization;
using System.Windows.Controls;

namespace VKRClient.Classes
{
    public class AttendanceStringConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            Grid Grid = new Grid();
            string? AttendanceString = values[0].ToString();

            if (string.IsNullOrEmpty(AttendanceString))
            {
                return new Border();
            }
            else
            {
                char[] array = AttendanceString.ToCharArray();
                Array.Reverse(array);
                AttendanceString = new string(array);
            }

            for (int i = 0; i < AttendanceString.Length; i++)
            {
                Grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

                Border Border = new Border
                {
                    Background = new SolidColorBrush(GetColorForDigit(AttendanceString[i])),
                    BorderThickness = new Thickness(0)
                };

                Grid.SetColumn(Border, i);
                Grid.Children.Add(Border);
            }

            return Grid;
        }

        private Color GetColorForDigit(char digit)
        {
            switch (digit)
            {
                case '0': return (Color)ColorConverter.ConvertFromString("#FF0000");
                case '1': return (Color)ColorConverter.ConvertFromString("#ff8b00");
                case '2': return (Color)ColorConverter.ConvertFromString("#00FF00");
                default: return Colors.Transparent;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
