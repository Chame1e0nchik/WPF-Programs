using System.Windows;
using Microsoft.Win32;
using System.IO;

namespace Lab2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string? filepath;
        private WorkSpace? space;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void OnNewClick(object sender, RoutedEventArgs e)
        {
            LoadContent("");
        }

        private void OnOpenClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = false;

            if (openFileDialog.ShowDialog() == true)
            {
                filepath = openFileDialog.FileName; // file path
                string fileContent = File.ReadAllText(filepath);
                LoadContent(fileContent);
            }
        }

        private void OnSaveClick(object sender, RoutedEventArgs e)
        {
            if (filepath != null && space != null)
            {
                SaveItem.IsEnabled = true;
                File.WriteAllText(filepath, space.TextField.Text);
            }
        }

        private void OnCopyClick(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(space?.TextField.SelectedText))
            {
                space.TextField.Copy();
            }
        }

        private void OnPasteClick(object sender, RoutedEventArgs e)
        {
            space?.TextField.Paste();
        }

        private void LoadContent(string text)
        {
            space = new WorkSpace(text);
            SaveItem.IsEnabled = true;
            WorkSpaceContent.Content = space;
        }
    }
}
