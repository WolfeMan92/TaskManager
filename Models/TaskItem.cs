using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManager.Models
{
    public class TaskItem
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Title { get; set; }

        public string? Description { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DueDate { get; set; }

        public string Status { get; set; } = "Pending";

        public string Priority { get; set; } = "Normal";

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public ICollection<TaskCategory> TaskCategories { get; set; }

    }
}