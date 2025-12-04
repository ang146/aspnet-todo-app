namespace TodoApp.Managers
{
    public interface IUserManager
    {
        Guid GetCurrentUserIdByHttpContext();
        Task<Guid> GetCurrentUserIdByStateAsync();

    }
}
