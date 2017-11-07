using System.Data.Entity;

namespace Rustic.Models
{
    /// <summary>
    /// Контекст для работы с базой
    /// </summary>
    public class TaskContext : DbContext
    {
        /// <summary>
        /// Задачи
        /// </summary>
        public DbSet<TaskItem> Tasks { get; set; }

        /// <summary>
        /// Пользователи
        /// </summary>
        public DbSet<User> Users { get; set; }

        public TaskContext() : base("DefaultConnection")
        {

        }
    }
}
