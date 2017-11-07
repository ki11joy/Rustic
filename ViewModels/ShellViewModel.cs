using Caliburn.Micro;
using Rustic.DataService;
using Rustic.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Rustic.ViewModels
{
    public class ShellViewModel : Screen
    {
        private IRusticClient _rusticClient;
        private TaskItemViewModel _selectedTaskCollection;
        private bool _isUIBlocked;
        private string _status;
        private bool _showByGroup = true;
        private bool _showIsDone = false;
        private ObservableCollection<TaskItemViewModel> _tasks = new ObservableCollection<TaskItemViewModel>();
        private IEnumerable<string> _groups;

        /// <summary>
        /// Флаг блокировки интерфейса во время аснхронной задачи.
        /// Вообще говоря асинхронность тут не очень и нужна, но пусть будет
        /// </summary>
        public bool IsUIBlocked
        {
            get => _isUIBlocked;
            set
            {
                _isUIBlocked = value;
                NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Группировать задачи
        /// </summary>
        public bool ShowByGroup
        {
            get => _showByGroup;
            set
            {
                _showByGroup = value;
                if (_showByGroup)
                    TaskCollection.GroupDescriptions.Add(new PropertyGroupDescription("Group"));
                else
                    TaskCollection.GroupDescriptions.Clear();
                NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Показывать завершенные
        /// </summary>
        public bool ShowIsDone
        {
            get => _showIsDone;
            set
            {
                _showIsDone = value;
                NotifyOfPropertyChange();
                TaskCollection.Refresh();
            }
        }

        /// <summary>
        /// Коллекция задач
        /// </summary>
        public ICollectionView TaskCollection => CollectionViewSource.GetDefaultView(_tasks);

        /// <summary>
        /// Выбранная задача
        /// </summary>
        public TaskItemViewModel SelectedTaskCollection
        {
            get => _selectedTaskCollection;
            set
            {
                if (_selectedTaskCollection?.IsChanged ?? false)
                {
                    _rusticClient.Update(_selectedTaskCollection.TaskItem).Wait();
                    TaskCollection.Refresh();
                }

                if (_selectedTaskCollection != value)
                {
                    _selectedTaskCollection = value;
                    ApplyGroups();
                    NotifyOfPropertyChange();
                }
            }
        }

        /// <summary>
        /// Строка статуса
        /// </summary>
        public string Status
        {
            get => _status;
            set
            {
                _status = value;
                NotifyOfPropertyChange();
            }
        }


        /// <summary>
        /// Флаг коннекта
        /// </summary>
        public bool IsConnected => _rusticClient.IsConnected;

        public ShellViewModel(IRusticClient rusticClient)
        {
            _rusticClient = rusticClient;
            Status = Localization.Strings.ShellViewModel_StatusDisconnected;
            Activated += ShellViewModel_Activated;
            TaskCollection.Filter = Filter;
        }

        /// <summary>
        /// Команда обновить список
        /// </summary>
        public async void RefreshList()
        {
            await DoRefreshList();
        }

        protected async Task DoRefreshList()
        {
            try
            {
                IsUIBlocked = true;

                int? id = SelectedTaskCollection?.Id;
                Status = Localization.Strings.ShellViewModel_StatusOperation;

                // запрашиваем список задач
                await FillTaskCollectionAsync();
                // запрашиваем группы
                _groups = await _rusticClient.GetGroups();
                ApplyGroups();


                // встаем на задачу, которая была активна до обновления или на первую в списке
                if (_tasks.Count > 0)
                {
                    if (id != null)
                        SelectedTaskCollection = _tasks.FirstOrDefault(o => o.Id == id);

                    if (SelectedTaskCollection == null)
                        SelectedTaskCollection = _tasks[0];
                }

                Status = Localization.Strings.ShellViewModel_StatusConnected;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                throw;
            }
            finally
            {
                IsUIBlocked = false;
            }
        }

        /// <summary>
        /// Команда добавить задачу
        /// </summary>
        public async void AddTask()
        {
            try
            {
                IsUIBlocked = true;
                Status = Localization.Strings.ShellViewModel_StatusCreating;
                // добавляем новую задачу сразу в базу
                var task = new TaskItem
                {
                    Created = DateTime.Now,
                    Title = Localization.Strings.ShellViewModel_NewTask
                };
                await _rusticClient.CreateTaskAsync(task);

                // добавляем ее в список и встаем на нее
                var taskItemViewModel = new TaskItemViewModel(task);
                _tasks.Add(taskItemViewModel);
                SelectedTaskCollection = taskItemViewModel;
                Status = string.Empty;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                IsUIBlocked = false;
            }
        }

        /// <summary>
        /// Команда отключиться от сервера
        /// </summary>
        /// <returns></returns>
        public async Task Logout()
        {
            await Disconnect();
        }

        /// <summary>
        /// Команда подклчиться к серверу
        /// </summary>
        public async void Login()
        {
            var manager = new WindowManager();
            var login = new LoginViewModel
            {
                Parent = this,
                Login = Properties.Settings.Default.Login,
                Password = Properties.Settings.Default.Password,
                RememberMe = Properties.Settings.Default.RememberMe
            };

            if (manager.ShowDialog(login) == true)
            {
                Properties.Settings.Default.Login = login.Login;
                Properties.Settings.Default.Password = login.Password;
                Properties.Settings.Default.RememberMe = login.RememberMe;
                Properties.Settings.Default.Save();

                await Connect(login.Login, login.Password);
                await DoRefreshList();
            }
        }

        private async Task Disconnect()
        {
            try
            {
                IsUIBlocked = true;
                Status = Localization.Strings.ShellViewModel_StatusOperation;
                await _rusticClient.DisconnectAsync();
                NotifyOfPropertyChange(nameof(IsConnected));
                Status = Localization.Strings.ShellViewModel_StatusDisconnected;
            }
            catch (Exception e)
            {
                MessageBox.Show(Localization.Strings.ShellViewModel_ConnectError +
                    Environment.NewLine +
                    e.Message);
                throw;
            }
            finally
            {
                IsUIBlocked = false;
            }
        }


        private async Task Connect(string login, string password)
        {
            try
            {
                IsUIBlocked = true;
                Status = Localization.Strings.ShellViewModel_StatusOperation;
                await _rusticClient.ConnectAsync(login, password);
                NotifyOfPropertyChange(nameof(IsConnected));
                Status = Localization.Strings.ShellViewModel_StatusConnected;
            }
            catch (Exception e)
            {
                MessageBox.Show(Localization.Strings.ShellViewModel_ConnectError +
                    Environment.NewLine +
                    e.Message);
                throw;
            }
            finally
            {
                IsUIBlocked = false;
            }
        }

        /// <summary>
        /// Фильтруем задачи по признаку завершенности локально, просто показать, что такое возможно
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private bool Filter(object obj)
        {
            var task = (TaskItemViewModel)obj;
            return ShowIsDone || (!ShowIsDone && !task.IsDone);
        }

        private async void ShellViewModel_Activated(object sender, ActivationEventArgs e)
        {
            if (Properties.Settings.Default.RememberMe)
            {
                await Connect(Properties.Settings.Default.Login, Properties.Settings.Default.Password);
                await DoRefreshList();
            }
        }

        /// <summary>
        /// Формирование списка задач
        /// </summary>
        /// <returns></returns>
        private async Task FillTaskCollectionAsync()
        {
            _tasks.Clear();
            TaskCollection.GroupDescriptions.Clear();
            IEnumerable<TaskItem> tasks = await _rusticClient.GetTaskItemsAsync();
            foreach (TaskItem task in tasks)
            {
                var taskItemViewModel = new TaskItemViewModel(task);
                _tasks.Add(taskItemViewModel);
            }
            if (ShowByGroup)
                TaskCollection.GroupDescriptions.Add(new PropertyGroupDescription("Group"));

            TaskCollection.Refresh();
        }

        private void ApplyGroups()
        {
            if (SelectedTaskCollection != null && _groups != null)
            {
                SelectedTaskCollection.Groups.Clear();
                foreach (string group in _groups)
                    SelectedTaskCollection.Groups.Add(group);
            }
        }
    }
}
