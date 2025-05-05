using System.ComponentModel.DataAnnotations;

namespace TaskManager.Models
{

        public class Category
        {
            public int Id { get; set; }

            [Required, StringLength(50)]
            public string Name { get; set; }

            public ICollection<TaskCategory> TaskCategories { get; set; }
        }
    }