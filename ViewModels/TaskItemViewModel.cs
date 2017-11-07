using Caliburn.Micro;
using Rustic.Models;
using System;
using System.Collections.ObjectModel;

namespace Rustic.ViewModels
{

    /// <summary>
    /// Вьюмодель для работы с задачей
    /// </summary>
    public class TaskItemViewModel : PropertyChangedBase
    {
        private TaskItem _task;
        private bool _iChanged;

        public TaskItem TaskItem => _task;

        public ObservableCollection<string> Groups { get; set; } = new ObservableCollection<string>();

        public bool IsChanged
        {
            get => _iChanged;
            set
            {
                _iChanged = value;
                NotifyOfPropertyChange();
            }
        }

        public int Id
        {
            get => _task.Id;
            set
            {
                _task.Id = value;
                _iChanged = true;
                NotifyOfPropertyChange();
            }
        }

        public string Title
        {
            get => _task.Title;
            set
            {
                _task.Title = value;
                _iChanged = true;
                NotifyOfPropertyChange();
            }
        }

        public string Description
        {
            get => _task.Description;
            set
            {
                _task.Description = value;
                _iChanged = true;
                NotifyOfPropertyChange();
            }
        }

        public string Group
        {
            get => _task.Group;
            set
            {
                if (value == "")
                    value = null;
                _task.Group = value;
                _iChanged = true;
                NotifyOfPropertyChange();
            }
        }

        public bool IsDone
        {
            get => _task.IsDone;
            set
            {
                _task.IsDone = value;
                _iChanged = true;
                NotifyOfPropertyChange();
            }
        }

        public DateTime? DeathLine
        {
            get => _task.DeathLine;
            set
            {
                _task.DeathLine = value;
                _iChanged = true;
                NotifyOfPropertyChange();
            }
        }



        public TaskItemViewModel(TaskItem task)
        {
            _task = task;
        }
    }
}
