using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace E2Edit
{
    /// <summary>
    /// Interaction logic for SaveAsDialog.xaml
    /// </summary>
    public partial class SaveAsDialog : Window
    {
        public string FileName { get; set; }

        public SaveAsDialog()
        {
            InitializeComponent();
            DataContext = this;
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Close, OnOk));
        }

        private void OnOk(object sender, ExecutedRoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void DoDragMove(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
