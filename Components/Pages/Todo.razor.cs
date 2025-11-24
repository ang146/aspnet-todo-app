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
  
    private readonly List<ITodoItemState> todos = new();

    private async Task RefreshTodoList()
    {
        todos.Clear();

        var dtos = await TodoService.GetItemsAsync();
        foreach (var dto in dtos)
        {
            todos.Add(TodoItemMapper.ToViewModel(dto));
        }
    }

    protected override async Task OnInitializedAsync()
    {
        await RefreshTodoList();
        await base.OnInitializedAsync();
    }

    public async Task SaveItems(){
        foreach (var toSave in todos.Where(t => t.IsDirty))
        {
            // Something to notify user saving process started, disabling UI
            var dto = TodoItemMapper.ToEntity(toSave);
            await TodoService.UpdateItemAsync(dto);
            toSave.SetCleanState();
            // Release UI lock, notify user saving process ended
        }
    }

    public async Task AddTodo(){
        var newTodo = TodoItemFactory.CreateNewTodoItem();

        var parameters = new DialogParameters<AddNewTodoDialog> { 
            { dlg => dlg.Item, newTodo } 
        };

        var dialog = await DialogService.ShowAsync<AddNewTodoDialog>("Add new Todo Item", parameters);
        var result = await dialog.Result;

        if (!result?.Canceled ?? false)
        {
            newTodo = result.Data as ITodoItemState;
            if (newTodo == null)
                return;

            var currentState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var currentUser = await UserManager.GetUserAsync(currentState.User);
            if (currentUser?.Id == null)
            {
                return;
            }
            newTodo.UserId = Guid.Parse(currentUser.Id);

            var dto = TodoItemMapper.ToEntity(newTodo);
            await TodoService.AddItemAsync(dto);
            await RefreshTodoList();
        }
    }

    public async Task UpdateTodo(TodoItem item)
    {
        await TodoService.UpdateItemAsync(item);
        await RefreshTodoList();
    }

    public async Task RemoveTodo(TodoItem toRemove)
    {
        await TodoService.DeleteItemAsync(toRemove.Id);
        await RefreshTodoList();
    }
}
