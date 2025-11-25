using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Identity;
using Microsoft.JSInterop;
using MudBlazor;
using System;
using TodoApp.Components.Dialogs;
using TodoApp.Data;
using TodoApp.Enums;
using TodoApp.Factories;
using TodoApp.Mappers;
using TodoApp.Models;
using TodoApp.Services;

namespace TodoApp.Components.Pages;

public partial class Todo : ComponentBase
{
    [Inject]
    public ITodoService TodoService { get; set; }
    [Inject]
    public AuthenticationStateProvider AuthenticationStateProvider { get; set; }
    [Inject]
    public UserManager<ApplicationUser> UserManager { get; set; }
    [Inject]
    public IHttpContextAccessor HttpContextAccessor { get; set; }
    [Inject]
    public ITodoItemFactory TodoItemFactory { get; set; }
    [Inject]
    public ITodoItemMapper TodoItemMapper { get; set; }
    [Inject]
    public IDialogService DialogService { get; set; }
    public IEnumerable<ITodoItemState> Todos { get; private set; } = new List<ITodoItemState>();

    private readonly List<ITodoItemState> _todos = new();
    private SortingMethod _sortingMethod;

    private async Task RefreshTodoList()
    {
        _todos.Clear();

        var dtos = await TodoService.GetItemsAsync();
        foreach (var dto in dtos)
        {
            _todos.Add(TodoItemMapper.ToViewModel(dto));
        }
        UpdateSorting();
    }

    private void UpdateSorting()
    {
        List<ITodoItemState> todos;
        switch (SortingMethod) {
            case SortingMethod.IsDone:
                todos = _todos.OrderBy(t => t.IsDone)
                    .ThenByDescending(t => t.Priority)
                    .ThenBy(t => t.Title)
                    .ToList();
                break;
            case SortingMethod.Alphabet:
                todos = _todos.OrderBy(t => t.Title)
                    .ThenByDescending(t => t.Priority)
                    .ToList();
                break;
            case SortingMethod.Priority:
                todos = _todos.OrderByDescending(t => t.Priority)
                    .ThenBy(t=> t.Title)
                    .ToList();
                break;
            case SortingMethod.Deadline:
                todos = _todos.OrderBy(t => t.Deadline)
                    .ThenByDescending(t => t.Priority)
                    .ThenBy(t => t.Title)
                    .ToList();
                break;
            default:
                throw new NotImplementedException();
        }

        Todos = todos;
    }

    protected override async Task OnInitializedAsync()
    {
        await RefreshTodoList();
        await base.OnInitializedAsync();
    }

    public async Task SaveItems()
    {
        // Something to notify user saving process started, disabling UI
        foreach (var toDelete in _todos.Where(t => t.ModificationState == ModificationState.Delete))
        {
            await TodoService.DeleteItemAsync(toDelete.Id);
        }

        foreach (var toSave in _todos.Where(t => t.IsDirty))
        {
            var dto = TodoItemMapper.ToEntity(toSave);
            await TodoService.UpdateItemAsync(dto);
            toSave.SetCleanState();
        }

        // Release UI lock, notify user saving process ended
        await RefreshTodoList();
    }

    public async Task EditTodo(ITodoItemState vm)
    {
        var title = "Editing existing Todo";
        var parameters = new DialogParameters<TodoModificationDialog>
        {
            {dlg => dlg.Item, vm },
            {dlg => dlg.Title, title }
        };

        var dialog = await DialogService.ShowAsync<TodoModificationDialog>(title, parameters);
        var result = await dialog.Result;

        if (result?.Canceled ?? true)
            return;

    }

    public async Task AddTodo(){
        var newTodo = TodoItemFactory.CreateNewTodoItem();

        var title = "Adding new Todo";
        var parameters = new DialogParameters<TodoModificationDialog> { 
            { dlg => dlg.Item, newTodo } ,
            {dlg => dlg.Title, title }
        };

        var dialog = await DialogService.ShowAsync<TodoModificationDialog>(title, parameters);
        var result = await dialog.Result;

        if (result?.Canceled ?? true)
            return;

        var currentState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        var currentUser = await UserManager.GetUserAsync(currentState.User);
        if (currentUser?.Id == null)
            return;

        newTodo.UserId = Guid.Parse(currentUser.Id);

        /* TODO: Move to Save method. */
        var dto = TodoItemMapper.ToEntity(newTodo);
        await TodoService.AddItemAsync(dto);
        await RefreshTodoList();
    }

    public async Task DeleteTodo(ITodoItemState vm)
    {
        vm.SetDelete();
    }

    public SortingMethod SortingMethod {
        get => _sortingMethod;
        set
        {
            _sortingMethod = value;
            UpdateSorting();
        }
    }
}
