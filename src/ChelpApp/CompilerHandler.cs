using ChelpApp.Compilation;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Linq;

namespace ChelpApp
{
    public partial class MainWindow : Window
    {
        List<Compiler> Compilers { get; set; } = [];

        Compiler? chosenCompiler = null;
        string? CompilerArgs { get; set; } = string.Empty;

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
            chosenCompiler = null;
            var elementsToRemove = sp_CompilerList.Children.OfType<RadioButton>().ToList();
            foreach (var element in elementsToRemove)
            {
                sp_CompilerList.Children.Remove(element);
            }
            label_CompilerStatus.Content = "Поиск и тестирование компиляторов...";
            pb_CompilersSearchingAndTesting.IsIndeterminate = true;
            button_RefreshCompilers.IsEnabled = false;

            List<Compiler> compilers = await CompilerHandler.SearchAndTest();

            if (compilers.Count == 0)
            {
                label_CompilerStatus.Content = "Ошибка";
                pb_CompilersSearchingAndTesting.IsIndeterminate = false;
                pb_CompilersSearchingAndTesting.Foreground = Brushes.Red;
                MessageBox.Show("На компьютере нет доступных компиляторов!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
                return;
            }

            Compilers = compilers;
            label_CompilerStatus.Content = "Завершено";
            pb_CompilersSearchingAndTesting.IsIndeterminate = false;
            button_RefreshCompilers.IsEnabled = true;
            foreach (var compiler in compilers)
            {
                RadioButton compilerRB = new()
                {
                    Content = $"{compiler.Description}",
                    IsEnabled = compiler.IsValid,
                    Margin = new(5, 5, 5, 5),
                    ToolTip = $"Исполняемый файл: {compiler.Name} ({compiler.Fullpath}), версия: {compiler.Version}",
                };
                ToolTipService.SetShowOnDisabled(compilerRB, true);
                compilerRB.Checked += SetChosenCompiler;
                sp_CompilerList.Children.Add(compilerRB);
            };
        }

        private void SetChosenCompiler(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton)
            {
                int index = sp_CompilerList.Children.IndexOf(sender as RadioButton);
                chosenCompiler = Compilers[index-1];
                Debugger.Debug.Log($"chosen compiler: {chosenCompiler?.Fullpath ?? "none"}");
                return;
            }
        }

        public static void ResetCache()
        {
            CompilerFinder.ResetCache();
        }
    }
}
