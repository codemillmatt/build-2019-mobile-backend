using System;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace TailwindTraders.Mobile.Features.LogIn
{
    public class FakeAuthenticationService : IAuthenticationService
    {
        public string AuthorizationHeader => string.Empty;

        public bool IsAnyOneLoggedIn { get; private set; }

        public async Task<string> GetUserName()
        {
            return (await SecureStorage.GetAsync("username")).ToLower();
        }

        public async Task LogInAsync(string email, string password)
        {
            await SecureStorage.SetAsync("username", email.ToLower());

            IsAnyOneLoggedIn = true;
        }

        public async Task LogInWithMicrosoftAsync()
        {
            await Task.Delay(TimeSpan.FromSeconds(2));

            IsAnyOneLoggedIn = true;
        }

        public void LogOut()
        {
            IsAnyOneLoggedIn = false;
        }

        public async Task RefreshSessionAsync()
        {           
            var userName = await SecureStorage.GetAsync("username");

            IsAnyOneLoggedIn = !string.IsNullOrEmpty(userName);
        }
    }
}