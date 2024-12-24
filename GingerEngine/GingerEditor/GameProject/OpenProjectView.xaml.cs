using System.Windows;
using System.Windows.Controls;

namespace GingerEditor.GameProject
{
    /// <summary>
    /// Interaction logic for OpenProjectView.xaml
    /// </summary>
    public partial class OpenProjectView : UserControl
    {
        public OpenProjectView()
        {
            InitializeComponent();

            Loaded += (s, e) =>
            {
                var item = projectListBox.ItemContainerGenerator.ContainerFromIndex(projectListBox.SelectedIndex) as ListBoxItem;
                item?.Focus();
            };
        }

        private void OnOpenButton_Click(object sender, RoutedEventArgs e)
        {
            OpenSelectedProject();
        }

        private void OnProject_DoubleClick(object sender, RoutedEventArgs e)
        {
            OpenSelectedProject();
        }

        private void OpenSelectedProject()
        {
            var project = OpenProject.Open(projectListBox.SelectedItem as ProjectData);

            bool dialogResult = false;
            var win = Window.GetWindow(this);

            if (project != null)
            {
                dialogResult = true;
                win.DataContext = project;
            }

            win.DialogResult = dialogResult;
            win.Close();
        }
    }
}
