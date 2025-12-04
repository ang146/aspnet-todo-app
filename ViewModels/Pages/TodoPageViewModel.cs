using MudBlazor;
using TodoApp.Enums;
using TodoApp.Managers;
using TodoApp.Mappers;
using TodoApp.Models;
using TodoApp.Services;

namespace TodoApp.ViewModels.Pages
{
    public class TodoPageViewModel : ITodoPageViewModel
    {
        private readonly ITodoService _todoService;
        private readonly ITodoItemMapper _todoItemMapper;
        private readonly IUserManager _userManager;

        public TodoPageViewModel(ITodoService todoService,
            ITodoItemMapper todoItemMapper,
            IUserManager userManager)
        {
            _todoService = todoService;
            _userManager = userManager;
            _todoItemMapper = todoItemMapper;
        }

        public async Task InitialiseAsync()
        {
            await RefreshTodoListAsync();
        }

        public async Task RefreshTodoListAsync()
        {
            _todos.Clear();

            var dtos = await _todoService.GetItemsAsync();
            foreach (var dto in dtos)
            {
                _todos.Add(_todoItemMapper.ToViewModel(dto));
            }
            UpdateSortingAndFiltering();
        }

        private void UpdateSortingAndFiltering()
        {
            List<ITodoItemState> todos;

            var filteredTodos = _todos.ToList();
            if (HideDone)
            {
                filteredTodos.RemoveAll(t => t.IsDone);
            }

            if (HideOverdue)
            {
                filteredTodos.RemoveAll(t => t.Deadline <= DateTime.Today);
            }

            if (!string.IsNullOrWhiteSpace(ShowCategory))
            {
                Category category = Enum.Parse<Category>(ShowCategory);
                filteredTodos = filteredTodos.Where(t => t.Category == category).ToList();
            }

            switch (SortingMethod)
            {
                case SortingMethod.IsDone:
                    todos = filteredTodos.OrderBy(t => t.IsDone)
                        .ThenByDescending(t => t.Priority)
                        .ThenBy(t => t.Title)
                        .ToList();
                    break;
                case SortingMethod.Alphabet:
                    todos = filteredTodos.OrderBy(t => t.Title)
                        .ThenByDescending(t => t.Priority)
                        .ToList();
                    break;
                case SortingMethod.Priority:
                    todos = filteredTodos.OrderByDescending(t => t.Priority)
                        .ThenBy(t => t.Title)
                        .ToList();
                    break;
                case SortingMethod.Deadline:
                    todos = filteredTodos.OrderBy(t => t.Deadline)
                        .ThenByDescending(t => t.Priority)
                        .ThenBy(t => t.Title)
                        .ToList();
                    break;
                default:
                    throw new NotImplementedException();
            }

            Todos = todos;
        }

        public IEnumerable<ITodoItemState> Todos { get; private set; }

        private readonly List<ITodoItemState> _todos = new();
        private SortingMethod _sortingMethod;
        private bool _hideDone = false;
        private bool _hideOverdue = true;
        private string _showCategory = string.Empty;

        public SortingMethod SortingMethod
        {
            get => _sortingMethod;
            set
            {
                _sortingMethod = value;
                UpdateSortingAndFiltering();
            }
        }

        public bool HideDone
        {
            get => _hideDone;
            set
            {
                _hideDone = value;
                UpdateSortingAndFiltering();
            }
        }

        public bool HideOverdue
        {
            get => _hideOverdue;
            set
            {
                _hideOverdue = value;
                UpdateSortingAndFiltering();
            }
        }

        public string ShowCategory
        {
            get => _showCategory;
            set
            {
                _showCategory = value;
                UpdateSortingAndFiltering();
            }
        }

        public bool IsCalendar { get; private set; }

        public async Task AddTask(ITodoItemState vm)
        {
            var userId = await _userManager.GetCurrentUserIdByStateAsync();
            vm.UserId = userId;

            _todos.Add(vm);
            UpdateSortingAndFiltering();
        }

        public Task DeleteTask(ITodoItemState vm)
        {
            if (vm.ModificationState == ModificationState.Added)
            {
                _todos.Remove(vm);
                UpdateSortingAndFiltering();
                return Task.CompletedTask;
            }

            vm.SetDelete();
            return Task.CompletedTask;
        }

        public async Task SaveTasks()
        {
            foreach (var toDelete in _todos.Where(t => t.ModificationState == ModificationState.Delete))
            {
                await _todoService.DeleteItemAsync(toDelete.Id);
            }

            foreach (var toAdd in _todos.Where(t => t.ModificationState == ModificationState.Added))
            {
                var dto = _todoItemMapper.ToEntity(toAdd);
                await _todoService.AddItemAsync(dto);
                toAdd.SetCleanState();
            }

            foreach (var toSave in _todos.Where(t => t.ModificationState == ModificationState.Edited))
            {
                var dto = _todoItemMapper.ToEntity(toSave);
                await _todoService.UpdateItemAsync(dto);
                toSave.SetCleanState();
            }

            await RefreshTodoListAsync();
        }

        public void ToggleCalendar()
        {
            IsCalendar = !IsCalendar;
        }

    }
}
