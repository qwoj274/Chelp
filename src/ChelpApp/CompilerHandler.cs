using ChelpApp.Compilation;
using System.Windows;
using System.Windows.Media;

namespace ChelpApp
{
    public partial class MainWindow : Window
    {
        List<Compiler> Compilers { get; set; } = [];

        public static class CompilerHandler
        {
            public static async Task<List<Compiler>> SearchAndTest()
            {
                var result = await Task.Run(()=> CompilerFinder.SearchForCompiler());
                return result;
            }
        }

        private async void GetCompilersAfterWindowLoaded(object sender, RoutedEventArgs e)
        {
            await GetCompilersAsync();
        }

        private async Task GetCompilersAsync()
        {
            listview_CompilersList.ItemsSource = null;
            listview_CompilersList.Items.Clear();
            label_CompilerStatus.Content = "Поиск и тестирование компиляторов...";
            pb_CompilersSearchingAndTesting.IsIndeterminate = true;
            button_RefreshCompilers.IsEnabled = false;

            List<Compiler> compilers = await CompilerHandler.SearchAndTest();

            if (compilers.Count == 0)
            {
                label_CompilerStatus.Content = "Ошибка";
                pb_CompilersSearchingAndTesting.IsIndeterminate= false;
                pb_CompilersSearchingAndTesting.Foreground = Brushes.Red;
                MessageBox.Show("На компьютере нет доступных компиляторов!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
                return;
            }

            Compilers = compilers.Distinct().ToList();
            label_CompilerStatus.Content = "Завершено";
            pb_CompilersSearchingAndTesting.IsIndeterminate = false;
            button_RefreshCompilers.IsEnabled = true;
            listview_CompilersList.ItemsSource = Compilers;
        }

        public void ResetCache()
        {
            CompilerFinder.ResetCache();
        }

    }
}
