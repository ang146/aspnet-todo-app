using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using MudBlazor;
using System.Globalization;
using TodoApp.Components.Dialogs;
using TodoApp.Components.Pages;
using TodoApp.Data;
using TodoApp.Enums;
using TodoApp.Factories;
using TodoApp.Mappers;
using TodoApp.Models;
using TodoApp.Services;

namespace TodoApp.ViewModels.Pages
{
    public class TodoPageViewModel : ITodoPageViewModel
    {
        private readonly ITodoService _todoService;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITodoItemFactory _todoItemFactory;
        private readonly ITodoItemMapper _todoItemMapper;

        public TodoPageViewModel(ITodoService todoService,
            AuthenticationStateProvider authenticationStateProvider,
            UserManager<ApplicationUser> userManager,
            ITodoItemFactory todoItemFactory,
            ITodoItemMapper todoItemMapper)
        {
            _todoService = todoService;
            _authenticationStateProvider = authenticationStateProvider;
            _userManager = userManager;
            _todoItemFactory = todoItemFactory;
            _todoItemMapper = todoItemMapper;
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
            var currentState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            var currentUser = await _userManager.GetUserAsync(currentState.User);
            if (currentUser?.Id == null)
                return;

            vm.UserId = Guid.Parse(currentUser.Id);

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
