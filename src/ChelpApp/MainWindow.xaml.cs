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

        List<string> cppFilesList = [];

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

            cppFilesList = cppFinder.GetCppFiles().Select(p => p.FullName).ToList();

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
                foreach (string file in cppFilesList)
                {
                    CheckBox listItem = new()
                    {
                        Content = Path.GetFileName(file),
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
            var arrayOfCpp = listbox_ListOfCpp.Items.OfType<CheckBox>().ToArray();
            return arrayOfCpp;
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

            using (Process explorer = new())
            {
                explorer.StartInfo = new()
                {
                    FileName = "explorer.exe",
                    Arguments = path
                };
                explorer.Start();
            }
        }

        private void button_OpenLog_Click(object sender, RoutedEventArgs e)
        {
            string path = Debugger.Debug.LogFileFullpath;
            if (!File.Exists(path))
            {
                return;
            }

            using (Process notepad = new())
            {
                notepad.StartInfo = new()
                {
                    FileName = "notepad",
                    Arguments = path
                };
                notepad.Start();
            }
        }

        private void button_ResetLog_Click(object sender, RoutedEventArgs e)
        {
            Debugger.Debug.ResetLog();
            textbox_Debug.Text = string.Empty;
        }

        private void OpenExplorerCausedByIncorrectProjectpath()
        {
            OpenFolderDialog dialog = new OpenFolderDialog();
            dialog.ShowDialog();

            textbox_ProjectPath.Text = dialog.FolderName;
            OnPathSelected();
        }

        private async Task CompileCppFiles()
        {
            if (chosenCompiler == null)
            {
                MessageBox.Show("Перед компиляцией выберите компилятор!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (projectPath == string.Empty)
            {
                var result = MessageBox.Show("Выберите путь до ваших .cpp файлов!", "Ошибка", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                if (result == MessageBoxResult.OK)
                {
                    OpenExplorerCausedByIncorrectProjectpath();
                }
                return;
            }

            if (!Path.Exists(projectPath))
            {
                MessageBox.Show("Выбранного пути не существует!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (cppFilesList.Count == 0)
            {
                MessageBox.Show("Не выбраны файлы для компиляции, или таких файлов нет в выбранной папке!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            string outputFilePath = Path.Combine(projectPath, "Out", "out.exe");
            int compilationErrorCode = await Task.Run(() => chosenCompiler.Compile(outputFilePath, null, cppFilesList));

            /*
             display progress bar, waiting for compilation finished
             */

            if  (compilationErrorCode != 0)
            {
                MessageBox.Show("Компиляция не удалась по неизвестной причине! Подробнее в log.txt...", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

                return;
            }

            Process runOutExecutable = new Process()
            {
                StartInfo = new()
                {
                    FileName = outputFilePath,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                }
            };

            using (runOutExecutable)
            {
                runOutExecutable.Start();
                string output = runOutExecutable.StandardOutput.ReadToEnd();
                string error = runOutExecutable.StandardError.ReadToEnd();
                runOutExecutable.WaitForExit();

            }
        }
    }
}