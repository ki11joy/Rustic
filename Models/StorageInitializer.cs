using System;
using System.Data.Entity;

namespace Rustic.Models
{
    /// <summary>
    /// Инициализатор базы
    /// </summary>
    public class StorageInitializer : DropCreateDatabaseIfModelChanges<TaskContext>
    {
        protected override void Seed(TaskContext context)
        {
            // заводим пользователя для демки
            context.Users.Add(new User
            {
                Login = "demo",
                Password = "demo"
            });

            // накидываем несколько задач для примера
            context.Tasks.Add(new TaskItem
            {
                Title = "Завершить тестовое задание",
                Created = DateTime.Now,
                Group = "Важное"
            });
            context.Tasks.Add(new TaskItem
            {
                Title = "Спасти мир",
                Created = DateTime.Now,
                Description = "Осторожно, не привлекать внимание санитаров!",
                Group = "Важное",
                IsDone = true
            });
            context.Tasks.Add(new TaskItem
            {
                Title = "Купить молока",
                Created = DateTime.Now,
                Group = "Магазин",
                IsDone = true
            });
            context.Tasks.Add(new TaskItem
            {
                Title = "Купить еще один майбах",
                Created = DateTime.Now,
                Group = "Магазин",
            });

            base.Seed(context);
        }
    }
}
