using Microsoft.AspNetCore.Components;
using MudBlazor;
using TodoApp.Components.Dialogs;
using TodoApp.Enums;
using TodoApp.Factories;
using TodoApp.Models;
using TodoApp.ViewModels.Pages;

namespace TodoApp.Components.Pages;

public partial class Todo : ComponentBase
{
    [Inject]
    public IDialogService DialogService { get; set; }
    [Inject]
    public ITodoPageViewModel ViewModel { get; set; }

    public IEnumerable<ITodoItemState> Todos => ViewModel.Todos;

    private async Task RefreshTodoList()
    {
        await ViewModel.RefreshTodoListAsync();
    }

    protected override async Task OnInitializedAsync()
    {
        await ViewModel.RefreshTodoListAsync();
        await base.OnInitializedAsync();
    }

    public async Task SaveItems()
    {
        // Something to notify user saving process started, disabling UI
        await ViewModel.SaveTasks();
        // Release UI lock, notify user saving process ended
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
        await dialog.Result;
    }

    public async Task AddTodo(){
        var title = "Adding new Todo";
        var parameters = new DialogParameters<TodoModificationDialog> { 
            { dlg => dlg.Item, null } ,
            {dlg => dlg.Title, title }
        };

        var dialog = await DialogService.ShowAsync<TodoModificationDialog>(title, parameters);
        var result = await dialog.Result;

        if (result?.Canceled ?? true)
            return;

        var newTodo = result.Data as ITodoItemState;
        if (newTodo == null)
            return;

        await ViewModel.AddTask(newTodo);
    }

    public void DeleteTodo(ITodoItemState vm)
    {
        ViewModel.DeleteTask(vm);
    }

    public void ToggleCalendar()
    {
        ViewModel.ToggleCalendar();
    }

    public bool IsCalendar => ViewModel.IsCalendar;

    public SortingMethod SortingMethod
    {
        get => ViewModel.SortingMethod;
        set => ViewModel.SortingMethod = value;
    }
    
    public bool HideDone
    {
        get => ViewModel.HideDone;
        set => ViewModel.HideDone = value;
    }

    public bool HideOverdue
    {
        get => ViewModel.HideOverdue;
        set => ViewModel.HideOverdue = value;
    }

    public string ShowCategory
    {
        get => ViewModel.ShowCategory;
        set => ViewModel.ShowCategory = value;
    }
}
