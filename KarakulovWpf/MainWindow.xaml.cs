using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using KarakulovLib;
using Microsoft.Win32;

namespace KarakulovWpf
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly TaskManager _taskManager = new TaskManager();
        private List<MenuItem> _selectedStatuses = new List<MenuItem>();
        private List<MenuItem> _selectedPriorities = new List<MenuItem>();
        private string _searchText;
        private string _currentFilePath;
        private ICollectionView _tasksView;

        public MainWindow()
        {
            InitializeComponent();
            SetupTaskListView();
            InitializeFilters();
        }

        private void SetupTaskListView()
        {
            _tasksView = CollectionViewSource.GetDefaultView(_taskManager.GetAllTasks());
            TasksListView.ItemsSource = _tasksView;
            UpdateStatus();
        }

        private void InitializeFilters()
        {
            MenuFilterStatusAll.IsChecked = true;
            MenuFilterPriorityAll.IsChecked = true;
        }

        private void UpdateStatus(string message = null)
        {
            StatusText.Text = $"Задач: {_taskManager.GetAllTasks().Count()}";
            if (!string.IsNullOrEmpty(message))
            {
                StatusText.Text += $" | {message}";
            }
        }
        private void MenuSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(_currentFilePath))
                {
                    MessageBox.Show("Файл не найден, выполните действия *Создать* или *Сохранить как*", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    _taskManager.SaveToFile(_currentFilePath);
                    UpdateStatus("Файл сохранен");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void MenuSaveAs_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog
            {
                Filter = "JSON files (*.json)|*.json",
                DefaultExt = ".json"
            };

            if (dialog.ShowDialog() == true)
            {
                _currentFilePath = dialog.FileName;
                _taskManager.SaveToFile(_currentFilePath);
                UpdateStatus("Файл сохранен");
            }
        }

        private void MenuCreate_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog
            {
                Filter = "JSON files (*.json)|*.json",
                DefaultExt = ".json"
            };

            if (dialog.ShowDialog() == true)
            {
                _currentFilePath = dialog.FileName;
                _taskManager.СreateFile(_currentFilePath);
                UpdateStatus("Файл создан");
            }
        }

        private void MenuLoad_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "JSON files (*.json)|*.json",
                DefaultExt = ".json"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    _taskManager.LoadFromFile(dialog.FileName);
                    _currentFilePath = dialog.FileName;
                    _tasksView.Refresh();
                    UpdateStatus("Файл загружен");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке: {ex.Message}", "Ошибка",
                                  MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void MenuExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void MenuAddTask_Click(object sender, RoutedEventArgs e)
        {
            var window = new CommandWindow { Owner = this };
            if (window.ShowDialog() == true)
            {
                _taskManager.AddTask(window.Task);
                _tasksView.Refresh();
                UpdateStatus($"Добавлена задача #{window.Task.Id}");
            }
        }

        private void MenuEditTask_Click(object sender, RoutedEventArgs e)
        {
            if (TasksListView.SelectedItem is Task selectedTask)
            {
                var window = new CommandWindow(selectedTask) { Owner = this };
                if (window.ShowDialog() == true)
                {
                    _taskManager.UpdateTask(window.Task);
                    _tasksView.Refresh();
                    UpdateStatus($"Обновлена задача #{window.Task.Id}");
                }
            }
            else
            {
                MessageBox.Show("Выберите задачу для редактирования", "Информация",
                              MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void MenuDeleteTask_Click(object sender, RoutedEventArgs e)
        {
            if (TasksListView.SelectedItem is Task selectedTask)
            {
                if (MessageBox.Show($"Удалить задачу '{selectedTask.Title}'?", "Подтверждение",
                                   MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    _taskManager.RemoveTask(selectedTask);
                    _tasksView.Refresh();
                    UpdateStatus($"Удалена задача #{selectedTask.Id}");
                }
            }
            else
            {
                MessageBox.Show("Выберите задачу для удаления", "Информация",
                              MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            _searchText = SearchTextBox.Text.ToLower();
            ApplyFilters();
            SearchStatusText.Text = $"Найдено задач: {_tasksView.Cast<object>().Count()}";
        }

        private void MenuFilterPriority_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = sender as MenuItem;
            if (menuItem == null) return;

            if (menuItem == MenuFilterPriorityAll)
            {
                var parent = menuItem.Parent as MenuItem;
                foreach (MenuItem item in parent.Items)
                {
                    if (item != menuItem && item.HasHeader)
                    {
                        item.IsChecked = false;
                    }
                }
                MenuFilterPriorityAll.IsChecked = true;
                _selectedPriorities.Clear();
            }
            else
            {
                if (menuItem.IsChecked == true)
                {
                    menuItem.IsChecked = false;
                    _selectedPriorities.Remove(menuItem);
                    if (_selectedPriorities.Count == 0)
                    {
                        MenuFilterPriorityAll.IsChecked = true;
                    }
                }
                else
                {
                    menuItem.IsChecked = true;
                    MenuFilterPriorityAll.IsChecked = false;
                    _selectedPriorities.Add(menuItem);
                }
            }

            ApplyFilters();
        }

        private void ApplyFilters()
        {
            _tasksView.Filter = item =>
            {
                var task = item as Task;
                if (task == null) return false;

                bool statusFilterPassed = _selectedStatuses.Count == 0 ||
                                         MenuFilterStatusAll.IsChecked ||
                                         _selectedStatuses.Any(menuItem =>
                                             GetStatusFromMenuItem(menuItem) == task.Status);

                bool priorityFilterPassed = _selectedPriorities.Count == 0 ||
                                          MenuFilterPriorityAll.IsChecked ||
                                          _selectedPriorities.Any(menuItem =>
                                              GetPriorityFromMenuItem(menuItem) == task.Priority);

                bool searchFilterPassed = string.IsNullOrEmpty(_searchText) ||
                                         (task.Title?.ToLower().Contains(_searchText) == true ||
                                         (task.Description?.ToLower().Contains(_searchText) == true));

                return statusFilterPassed && priorityFilterPassed && searchFilterPassed;
            };

            UpdateStatus($"Отфильтровано: {_tasksView.Cast<object>().Count()}");
        }

        private string GetStatusFromMenuItem(MenuItem menuItem)
        {
            if (menuItem == MenuFilterStatusNew) return "Создано";
            if (menuItem == MenuFilterStatusInProgress) return "Выполняется";
            if (menuItem == MenuFilterStatusOnHold) return "На паузе";
            if (menuItem == MenuFilterStatusCompleted) return "Завершено";
            if (menuItem == MenuFilterStatusCanceled) return "Отменено";
            throw new ArgumentException("Неизвестный menuItem", nameof(menuItem));
        }

        private string GetPriorityFromMenuItem(MenuItem menuItem)
        {
            if (menuItem == MenuFilterPriorityLow) return "Низкий";
            if (menuItem == MenuFilterPriorityMedium) return "Средний";
            if (menuItem == MenuFilterPriorityHigh) return "Высокий";
            if (menuItem == MenuFilterPriorityCritical) return "Критический";
            throw new ArgumentException("Неизвестный menuItem", nameof(menuItem));
        }

        private void MenuFilterStatus_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = sender as MenuItem;
            if (menuItem == null) return;

            if (menuItem == MenuFilterStatusAll)
            {
                var parent = menuItem.Parent as MenuItem;
                foreach (MenuItem item in parent.Items)
                {
                    if (item != menuItem && item.HasHeader)
                    {
                        item.IsChecked = false;
                    }
                }
                MenuFilterStatusAll.IsChecked = true;
                _selectedStatuses.Clear();
            }
            else
            {
                if (menuItem.IsChecked == true)
                {
                    menuItem.IsChecked = false;
                    _selectedStatuses.Remove(menuItem);
                    if (_selectedStatuses.Count == 0)
                    {
                        MenuFilterStatusAll.IsChecked = true;
                    }
                }
                else
                {
                    menuItem.IsChecked = true;
                    MenuFilterStatusAll.IsChecked = false;
                    _selectedStatuses.Add(menuItem);
                }
            }
            ApplyFilters();
        }

        private void MenuResetFilters_Click(object sender, RoutedEventArgs e)
        {
            if (MenuFilterStatusAll != null)
            {
                var parent = MenuFilterStatusAll.Parent as MenuItem;
                if (parent != null)
                {
                    foreach (MenuItem item in parent.Items)
                    {
                        if (item != MenuFilterStatusAll && item.HasHeader)
                        {
                            item.IsChecked = false;
                        }
                    }
                }

                MenuFilterStatusAll.IsChecked = true;
                _selectedStatuses.Clear();

                parent = MenuFilterPriorityAll.Parent as MenuItem;
                if (parent != null)
                {
                    foreach (MenuItem item in parent.Items)
                    {
                        if (item != MenuFilterPriorityAll && item.HasHeader)
                        {
                            item.IsChecked = false;
                        }
                    }
                }

                MenuFilterPriorityAll.IsChecked = true;
                _selectedPriorities.Clear();
                _searchText = null;
                SearchTextBox.Text = "";
                SearchStatusText.Text = "";
                ApplyFilters();
            }
        }

        private void MenuResetSearch_Click(object sender, RoutedEventArgs e)
        {
            _searchText = null;
            SearchTextBox.Text = "";
            SearchStatusText.Text = "";
            ApplyFilters();
        }
        private void MenuSave_Click_2(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog
            {
                Filter = "JSON files (*.json)|*.json",
                DefaultExt = ".json"
            };

            if (dialog.ShowDialog() == true)
            {
                _currentFilePath = dialog.FileName;
                _taskManager.SaveToFile(_currentFilePath);
                UpdateStatus("Файл сохранен");
            }

        }

        private void MenuReset_Click(object sender, RoutedEventArgs e)
        {
            SearchStatusText.Text = "";
            _searchText = null;
            ApplyFilters();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            _taskManager.Dispose();
            base.OnClosing(e);
        }
    }
}
