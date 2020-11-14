using System;
using System.Windows;
using System.Windows.Interop;
using TaskDialogApi;

namespace WpfNetCoreApp31DoesNotWork
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OpenTaskDialogButton_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                new TaskDialog().ShowDialog(new WindowInteropHelper(this).Handle);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}{Environment.NewLine}{Environment.NewLine}{ex}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
