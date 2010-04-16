using System.Windows;
using System.Windows.Input;

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
