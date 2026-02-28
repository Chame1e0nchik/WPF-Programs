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
        private WorkSpace? workSpace;

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
            if (string.IsNullOrEmpty(filepath))
            {
                SaveFileDialog dialog = new SaveFileDialog
                {
                    Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*",
                    DefaultExt = "txt"
                };

                bool? result = dialog.ShowDialog();

                if (result != true)
                    return;

                filepath = dialog.FileName;
            }

            File.WriteAllText(filepath, workSpace?.TextField.Text);
        }

        private void OnCutClick(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(workSpace?.TextField.SelectedText))
            {
                workSpace.TextField.Cut();
            }
        }

        private void OnCopyClick(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(workSpace?.TextField.SelectedText))
            {
                workSpace.TextField.Copy();
            }
        }

        private void OnPasteClick(object sender, RoutedEventArgs e)
        {
            workSpace?.TextField.Paste();
        }

        private void LoadContent(string text)
        {
            workSpace = new WorkSpace(text);
            SaveItem.IsEnabled = true;
            WorkSpaceContent.Content = workSpace;
        }
    }
}
