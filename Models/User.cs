using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Rustic.Models
{
    /// <summary>
    /// Пользователь
    /// </summary>
    public class User
    {
        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Логин пользователя
        /// </summary>
        [Required]
        public string Login { get; set; }

        /// <summary>
        /// Пароль пользователя
        /// Все храним в открытом виде за недостатком времени
        /// </summary>
        [Required]
        public string Password { get; set; }

        /// <summary>
        /// Список задач, заведенных пользователем
        /// </summary>
        public virtual ICollection<TaskItem> Tasks { get; set; }
    }
}
