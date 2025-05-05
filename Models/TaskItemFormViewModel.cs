using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TaskManager.Models
{
    public class TaskItemFormViewModel
    {
        public TaskItem TaskItem { get; set; } = new TaskItem();

        [Display(Name = "Categories")]
        public List<int> SelectedCategoryIds { get; set; } = new();

        [BindNever] 
        public MultiSelectList Categories { get; set; } = default!;
    }
}
