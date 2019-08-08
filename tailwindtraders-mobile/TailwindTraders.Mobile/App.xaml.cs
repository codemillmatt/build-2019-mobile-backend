using System.Threading.Tasks;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using TailwindTraders.Mobile.Features.Common;
using TailwindTraders.Mobile.Features.Logging;
using TailwindTraders.Mobile.Features.LogIn;
using TailwindTraders.Mobile.Features.Settings;
using TailwindTraders.Mobile.Features.Shell;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using MonkeyCache.SQLite;
using System;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace TailwindTraders.Mobile
{
    public partial class App
    {
        private static NavigableElement navigationRoot;

        public App()
        {
            InitializeComponent();

            // Initialize monkey cache
            Barrel.ApplicationId = "tailwind";

            RegisterServicesAndProviders();

            MainPage = new TheShell();
        }

        public static NavigableElement NavigationRoot
        {
            get => GetShellSection(navigationRoot) ?? navigationRoot;
            set => navigationRoot = value;
        }

        public static TheShell Shell => Current.MainPage as TheShell;

        protected override void OnStart()
        {
            base.OnStart();

            SetupPushNotifications();

            AppCenter.Start($"ios={DefaultSettings.AppCenteriOSSecret};" +
                  $"android={DefaultSettings.AppCenterAndroidSecret};",
                  typeof(Analytics), typeof(Crashes));
        }

        // It provides a navigatable section for elements which aren't explicitly defined within the Shell. For example,
        // ProductCategoryPage: it's accessed from the fly-out through a MenuItem but it doesn't belong to any section
        internal static ShellSection GetShellSection(Element element)
        {
            if (element == null)
            {
                return null;
            }

            var parent = element;
            var parentSection = parent as ShellSection;

            while (parentSection == null && parent != null)
            {
                parent = parent.Parent;
                parentSection = parent as ShellSection;
            }

            return parentSection;
        }

        internal static async Task NavigateBackAsync() => await NavigationRoot.Navigation.PopAsync();

        internal static async Task NavigateModallyBackAsync() => await NavigationRoot.Navigation.PopModalAsync();

        internal static async Task NavigateToAsync(Page page, bool closeFlyout = false)
        {
            if (closeFlyout)
            {
                await Shell.CloseFlyoutAsync();
            }

            await NavigationRoot.Navigation.PushAsync(page).ConfigureAwait(false);
        }

        internal static async Task NavigateModallyToAsync(Page page, bool animated = true)
        {
            await Shell.CloseFlyoutAsync();
            await NavigationRoot.Navigation.PushModalAsync(page, animated).ConfigureAwait(false);
        }

#pragma warning disable CS0162 
        private void RegisterServicesAndProviders()
        {
            if (DefaultSettings.UseDebugLogging)
            {
                DependencyService.Register<DebugLoggingService>();
            }
            else
            {
                DependencyService.Register<AppCenterLoggingService>();
            }

            if (DefaultSettings.UseFakeAPIs)
            {
                DependencyService.Register<FakeRestPoolService>();
            }
            else
            {
                DependencyService.Register<RestPoolService>();
            }

            if (DefaultSettings.UseFakeAuthentication)
            {
                DependencyService.Register<FakeAuthenticationService>();
            }
            else
            {
                DependencyService.Register<AuthenticationService>();
            }
        }
#pragma warning restore CS0162 

        private void SetupPushNotifications()
        {
            if (!AppCenter.Configured)
            {
                //Push.PushNotificationReceived += (sender, e) =>
                //{
                //    // Add the notification message and title to the message
                //    var summary = $"Push notification received:" +
                //                        $"\n\tNotification title: {e.Title}" +
                //                        $"\n\tMessage: {e.Message}";

                //    // If there is custom data associated with the notification,
                //    // print the entries
                //    if (e.CustomData != null)
                //    {
                //        summary += "\n\tCustom data:\n";
                //        foreach (var key in e.CustomData.Keys)
                //        {
                //            summary += $"\t\t{key} : {e.CustomData[key]}\n";
                //        }
                //    }

                //    // Send the notification summary to debug output
                //    System.Diagnostics.Debug.WriteLine(summary);
                //};
            }
        }
    }


}
