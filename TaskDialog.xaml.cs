using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LeshopeChatBotUI
{
    /// <summary>
    /// Interaction logic for TaskDialog.xaml
    /// </summary>
    public partial class TaskDialog : Window
    {
        public TaskItem Task { get; private set; }

        public TaskDialog()
        {
            InitializeComponent();
        }

        public TaskDialog(TaskItem existingTask) : this()
        {
            Task = existingTask;
            NameBox.Text = Task.Name;
            DescriptionBox.Text = Task.Description;
            ReminderPicker.SelectedDate = Task.Reminder;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameBox.Text))
            {
                MessageBox.Show("Task name cannot be empty.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (Task == null)
            {
                Task = new TaskItem();
            }

            Task.Name = NameBox.Text;
            Task.Description = DescriptionBox.Text;
            Task.Reminder = ReminderPicker.SelectedDate;

            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
