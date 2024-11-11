using GingerEditor.GameProject;
using System.ComponentModel;
using System.Windows;

namespace GingerEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Loaded += OnMainWindowLoaded;

            Closing += OnWindowClosing;
        }

        private void OnMainWindowLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnMainWindowLoaded;

            OpenProjectBrowser();
        }

        private void OnWindowClosing(object? sender, CancelEventArgs e)
        {
            Closing -= OnWindowClosing;
            Project.Current?.UnloadProject();
        }

        private void OpenProjectBrowser()
        {
            var projectBrowser = new ProjectBrowserDialog();
            if (projectBrowser.ShowDialog() == false || projectBrowser.DataContext == null)
            {
                Application.Current.Shutdown();
            }
            else
            {
                Project.Current?.UnloadProject();

                DataContext = projectBrowser.DataContext;
            }
        }
    }
}