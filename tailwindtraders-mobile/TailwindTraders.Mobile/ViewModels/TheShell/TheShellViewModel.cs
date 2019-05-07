using System.Collections.Generic;
using System.Windows.Input;
using Microsoft.AppCenter.Analytics;
using TailwindTraders.Mobile.Features.LogIn;
using TailwindTraders.Mobile.Features.Product.Category;
using TailwindTraders.Mobile.Features.Scanning.AR;
using TailwindTraders.Mobile.Features.Scanning.Photo;
using TailwindTraders.Mobile.Features.Settings;
using TailwindTraders.Mobile.Framework;

namespace TailwindTraders.Mobile.Features.Shell
{
    internal class TheShellViewModel : BaseViewModel
    {
        public ICommand PhotoCommand => new AsyncCommand(
            _ => App.NavigateModallyToAsync(new CameraPreviewTakePhotoPage(), animated: false));

        public ICommand ARCommand => new AsyncCommand(
            _ => App.NavigateToAsync(new CameraPreviewPage(), closeFlyout: true));

        public ICommand LogOutCommand => new AsyncCommand(_ => App.NavigateModallyToAsync(new LogInPage()));

        public ICommand ProductTypeCommand => new AsyncCommand(typeId =>
        {
            Analytics.TrackEvent(AnalyticEvents.HomePageName,
                new Dictionary<string, string>{
                    {AnalyticEvents.FromPageAreaKey, AnalyticEvents.FlyoutPageAreaName },
                    {AnalyticEvents.FromPageEventKey,AnalyticEvents.HomePageName },
                    {AnalyticEvents.ProductCategoryKey, typeId.ToString() }
                });

            return App.NavigateToAsync(new ProductCategoryPage(typeId as string), closeFlyout: true);
        });

        public ICommand ProfileCommand => FeatureNotAvailableCommand;

        public ICommand SettingsCommand => new AsyncCommand(
            _ => App.NavigateToAsync(new SettingsPage(), closeFlyout: true));
    }
}