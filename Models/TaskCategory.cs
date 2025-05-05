﻿
namespace TaskManager.Models
{
    public class TaskCategory
    {
        public int TaskItemId { get; set; }
        public TaskItem TaskItem { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
