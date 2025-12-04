using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using TodoApp.Data;

namespace TodoApp.Managers
{
    public class UserManager : IUserManager
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserManager(AuthenticationStateProvider authenticationStateProvider,
            UserManager<ApplicationUser> userManager, 
            IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _authenticationStateProvider = authenticationStateProvider;
        }

        public Guid GetCurrentUserIdByHttpContext()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null)
            {
                return Guid.Empty;
            }

            var userIdS = user.FindFirstValue(ClaimTypes.NameIdentifier);
            var success = Guid.TryParse(userIdS, out var userId);
            return success ? userId : Guid.Empty;
        }

        public async Task<Guid> GetCurrentUserIdByStateAsync()
        {
            var currentState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            var currentUser = await _userManager.GetUserAsync(currentState.User);
            if (currentUser?.Id == null)
                return Guid.Empty;

            return Guid.Parse(currentUser.Id);
        }
    }
}
