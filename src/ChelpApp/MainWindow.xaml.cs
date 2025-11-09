using ChelpApp.Compilation;
using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;


namespace ChelpApp
{ 
    public partial class MainWindow : Window
    {
        string projectPath = string.Empty;

        List<FileInfo> cppFilesList = [];

        public MainWindow()
        {
            InitializeComponent();
            Loaded += GetCompilersAfterWindowLoaded;
            Debugger.Debug.debugPublisher.DebugEvent += DebugWriteLine;
        }

        private void ProjectPathChoiceButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFolderDialog dialog = new OpenFolderDialog();
            dialog.ShowDialog();

            textbox_ProjectPath.Text = dialog.FolderName;
            OnPathSelected();
        }

        private void OnPathSelected()
        {
            UpdateListOfCpp();
            projectPath = textbox_ProjectPath.Text;
            CppFinder cppFinder = new(projectPath);

            cppFilesList = cppFinder.GetCppFiles();

            bool isNoCppInPath = cppFilesList.Count == 0;
            bool isProjectPathTextboxEmpty = textbox_ProjectPath.Text == string.Empty;

            checkbox_SelectAll.IsEnabled = !isNoCppInPath;

            if (!checkbox_SelectAll.IsEnabled)
            {
                checkbox_SelectAll.IsChecked = false;
            }

            if (isNoCppInPath && !isProjectPathTextboxEmpty && Path.Exists(projectPath))
            {
                label_noCppFiles.Visibility = Visibility.Visible;
            }
            else
            {
                label_noCppFiles.Visibility = Visibility.Hidden;
                foreach (FileInfo file in cppFilesList)
                {
                    CheckBox listItem = new()
                    {
                        Content = file.Name,
                    };
                    listbox_ListOfCpp.Items.Add(listItem);
                }
            }
        }

        private void UpdateListOfCpp()
        {
            if (listbox_ListOfCpp.Items.Count < 2)
            {
                return;
            }

            foreach (CheckBox item in GetAllCheckboxesExceptSelectAll())
            {
                if (item.Name == "checkbox_SelectAll")
                {
                    continue;
                }
                listbox_ListOfCpp.Items.Remove(item);
            }
        }

        private CheckBox[] GetAllCheckboxesExceptSelectAll()
        {
            var listOfCpp = new CheckBox[listbox_ListOfCpp.Items.Count];

            listbox_ListOfCpp.Items.CopyTo(listOfCpp, 0);

            if (listOfCpp.Length < 2)
            {
                return [];
            }

            return listOfCpp;
        }

        private void checkbox_SelectAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (var checkbox in GetAllCheckboxesExceptSelectAll())
            {
                checkbox.IsChecked = checkbox_SelectAll.IsChecked;
            }
        }

        private void ChelpWindow_Loaded(object sender, RoutedEventArgs e)
        {
            checkbox_SelectAll.IsChecked = false;
            checkbox_SelectAll.IsEnabled = false;
            label_noCppFiles.Visibility = Visibility.Hidden;
        }

        private void textbox_ProjectPath_TextChanged(object sender, TextChangedEventArgs e)
        {
            OnPathSelected();
        }

        private async void button_RefreshCompilers_Click(object sender, RoutedEventArgs e)
        {
            button_OpenCacheInExplorer.IsEnabled = false;
            button_ResetCache.IsEnabled = false;
            await GetCompilersAsync();
            button_OpenCacheInExplorer.IsEnabled = true;
            button_ResetCache.IsEnabled = true;
        }

        public void DebugWriteLine(string message)
        {
            textbox_Debug.Text += message + "\n";
            textbox_Debug.ScrollToEnd();
            sv_DebugScrollViewer.ScrollToEnd();
        }

        private void button_ResetCache_Click(object sender, RoutedEventArgs e)
        {
            ResetCache();
        }

        private void button_OpenCacheInExplorer_Click(object sender, RoutedEventArgs e)
        {
            string path = CompilerFinder.FullPathToCacheFile;
            if (!Directory.Exists(path))
            {
                return;
            }

            Process.Start("explorer.exe", path);
        }

        private void button_OpenLog_Click(object sender, RoutedEventArgs e)
        {
            string path = Debugger.Debug.LogFileFullpath;
            if (!File.Exists(path))
            {
                return;
            }

            Process.Start("notepad.exe", path);
        }

        private void button_ResetLog_Click(object sender, RoutedEventArgs e)
        {
            Debugger.Debug.ResetLog();
            textbox_Debug.Text = string.Empty;
        }
    } 
}