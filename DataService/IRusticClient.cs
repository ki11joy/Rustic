using Rustic.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rustic.DataService
{
    public interface IRusticClient
    {
        bool IsConnected { get; }

        Task ConnectAsync(string login, string password);

        Task DisconnectAsync();

        Task CreateTaskAsync(TaskItem task);

        Task<IEnumerable<TaskItem>> GetTaskItemsAsync();

        Task Update(TaskItem taskItem);

        Task<IEnumerable<string>> GetGroups();
    }
}
