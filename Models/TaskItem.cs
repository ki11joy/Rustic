using System;

namespace Rustic.Models
{
    /// <summary>
    /// Модель задачи для сохранения в базе
    /// </summary>
    public class TaskItem
    {
        /// <summary>
        /// Идентификатор объекта
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Связь с создавшим пользователем
        /// </summary>
        public virtual int UserId { get; set; }

        /// <summary>
        /// Заголовок задачи
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Описание задачи
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Флаг завершения
        /// </summary>
        public bool IsDone { get; set; }

        /// <summary>
        /// Группа, к которой относится задача
        /// </summary>
        public string Group { get; set; }

        public DateTime? DeathLine { get; set; }

        public DateTime Created { get; set; }
    }
}
