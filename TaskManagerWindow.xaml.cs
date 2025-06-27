using System.Collections.ObjectModel;
using System.Windows;


namespace LeshopeChatBotUI
{
    /// <summary>
    /// Interaction logic for TaskManagerWindow.xaml
    /// </summary>
    public partial class TaskManagerWindow : Window
    {
        private ObservableCollection<TaskItem> tasks = new ObservableCollection<TaskItem>();

        public TaskManagerWindow()
        {
            InitializeComponent();
            TaskListView.ItemsSource = tasks;
        }

        private void AddTask_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new TaskDialog();
            if (dialog.ShowDialog() == true)
            {
                tasks.Add(dialog.Task);
            }
        }

        private void EditTask_Click(object sender, RoutedEventArgs e)
        {
            if (TaskListView.SelectedItem is TaskItem selectedTask)
            {
                var dialog = new TaskDialog(selectedTask);
                if (dialog.ShowDialog() == true)
                {
                    // Refresh the list
                    TaskListView.Items.Refresh();
                }
            }
        }

        private void DeleteTask_Click(object sender, RoutedEventArgs e)
        {
            if (TaskListView.SelectedItem is TaskItem selectedTask)
            {
                tasks.Remove(selectedTask);
            }
        }
    }
}
