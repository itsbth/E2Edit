using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace E2Edit
{
    [ValueConversion(typeof(FileListItem), typeof(UIElement))]
    class FileListIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var b = (bool) value;
            return Resources.Resource.GetBitmapImage(b ? "E2.Icons.Folder" : "E2.Icons.Script");;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    class FileListItem
    {
        public bool IsFolder { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
    }
}
