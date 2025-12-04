using TodoApp.Enums;
using TodoApp.Models;

namespace TodoApp.ViewModels.Pages
{
    public interface ITodoPageViewModel
    {
        Task InitialiseAsync();
        Task RefreshTodoListAsync();
        Task SaveTasks();
        Task DeleteTask(ITodoItemState vm);
        Task AddTask(ITodoItemState vm);
        void ToggleCalendar();
        SortingMethod SortingMethod { get; set; }
        bool HideDone { get; set; }
        bool HideOverdue { get; set; }
        string ShowCategory { get; set; }
        bool IsCalendar { get; }
        IEnumerable<ITodoItemState> Todos { get; }
    }
}
