using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using KarakulovLib;

namespace KarakulovWpf
{
    /// <summary>
    /// Логика взаимодействия для CommandWindow.xaml
    /// </summary>
    public partial class CommandWindow : Window
    {
        private Task _originalTask;

        private Task _task;
        public Task Task
        {
            get => _task;
            set => _task = value;
        }

        public CommandWindow()
        {
            InitializeComponent();
            InitializeControls();
            SetupNewTask();
        }
        public CommandWindow(Task taskToEdit) : this()
        {
            if (taskToEdit == null)
                throw new ArgumentNullException(nameof(taskToEdit));

            _originalTask = taskToEdit;
            Task = taskToEdit.Clone();
            LoadTaskData();
        }

        private void InitializeControls()
        {

            PriorityComboBox.ItemsSource = new string[] { "Критический", "Высокий", "Средний", "Низкий" };
            StatusComboBox.ItemsSource = new string[] { "Создано", "Выполняется", "На паузе", "Завершено", "Отменено" };

            DueDatePicker.SelectedDate = DateTime.Today;
        }

        private void SetupNewTask()
        {
            if (Task == null)
            {
                Task = new Task
                {
                };
            }
        }

        private void LoadTaskData()
        {
            TitleTextBox.Text = Task.Title;
            DescriptionTextBox.Text = Task.Description;
            PriorityComboBox.SelectedItem = Task.Priority;
            StatusComboBox.SelectedItem = Task.Status;
            DueDatePicker.SelectedDate = Task.DueDate;
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(TitleTextBox.Text))
            {
                MessageBox.Show("Название задачи не может быть пустым", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                TitleTextBox.Focus();
                return false;
            }

            if (DueDatePicker.SelectedDate < DateTime.Today)
            {
                MessageBox.Show("Дата выполнения не может быть в прошлом", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                DueDatePicker.Focus();
                return false;
            }

            return true;
        }

        private void SaveTaskData()
        {
            Task.Title = TitleTextBox.Text.Trim();
            Task.Description = DescriptionTextBox.Text.Trim();
            Task.Priority = PriorityComboBox.SelectedItem.ToString();
            Task.Status = StatusComboBox.SelectedItem.ToString();
            Task.DueDate = DueDatePicker.SelectedDate;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput())
                return;

            SaveTaskData();

            if (_originalTask != null)
            {
                _originalTask.CopyFrom(Task);
            }

            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}