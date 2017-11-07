using Rustic.Extensions;
using Rustic.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Rustic.DataService
{
    /// <summary>
    /// Слой для работы с сервисом данных. Можно заменить на работу с каким-нибудь вебсервисом.
    /// </summary>
    public class RusticClient : IRusticClient
    {
        private TaskContext _context;
        private User _user;

        /// <summary>
        /// Пользователь подключен
        /// </summary>
        public bool IsConnected => _user != null;

        public RusticClient(TaskContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Подключиться к сервису данных
        /// </summary>
        /// <param name="login"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task ConnectAsync(string login, string password)
        {
            await Task.Delay(1000); // напряженно работаем

            _user = _context.Users.FirstOrDefault(o => o.Login == login && o.Password == password);
            if (!IsConnected)
                throw new Exception(Localization.Strings.UserNotFound);
        }

        /// <summary>
        /// Отключиться
        /// </summary>
        /// <returns></returns>
        public async Task DisconnectAsync()
        {
            await Task.Delay(1);
            _user = null;
        }

        /// <summary>
        /// Создать новую задачу
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public async Task CreateTaskAsync(TaskItem task)
        {
            task.UserId = _user.Id;
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
            await Task.Delay(3000); // напряженно работаем
        }

        /// <summary>
        /// Обновить существующую задачу
        /// </summary>
        /// <param name="taskItem"></param>
        public async Task Update(TaskItem taskItem)
        {
            await Task.Run(() =>
            {
                _context.Entry(taskItem).State = EntityState.Modified;
                _context.SaveChanges();
            }).ConfigureAwait(false);
        }

        /// <summary>
        /// Получить список задач
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<TaskItem>> GetTaskItemsAsync()
        {
            return await Task.Run(() =>
            {
                return _context.Tasks.Where(o => o.UserId == _user.Id).AsEnumerable();
            });
        }

        /// <summary>
        /// Получить группы, заведенные пользователем
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<string>> GetGroups()
        {
            return await Task.Run(async () =>
            {
                IEnumerable<TaskItem> list = await GetTaskItemsAsync();
                return list.Where(o => !o.Group.IsEmpty()).GroupBy(o => o.Group).Select(o => o.Key);
            });
        }
    }
}
