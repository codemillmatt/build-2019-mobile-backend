using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TailwindTraders.Mobile.Features.LogIn;
using TailwindTraders.Mobile.Features.Product;
using TailwindTraders.Mobile.Features.Product.Category;
using TailwindTraders.Mobile.Features.Scanning.AR;
using TailwindTraders.Mobile.Features.Scanning.Photo;
using TailwindTraders.Mobile.Framework;
using TailwindTraders.Mobile.Helpers;
using Xamarin.Forms;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using TailwindTraders.Mobile.Features.Settings;
using Microsoft.AppCenter.Crashes;

namespace TailwindTraders.Mobile.Features.Home
{
    public class HomeViewModel : BaseStateAwareViewModel<HomeViewModel.State>
    {
        public enum State
        {
            EverythingOK,
            Error,
        }

        private IEnumerable<Tuple<string, string, AsyncCommand>> recommendedProducts;
        private IEnumerable<ProductViewModel> popularProducts;
        private IEnumerable<ProductDTO> previouslySeenProducts;

        public HomeViewModel()
        {
            IsBusy = true;

            MessagingCenter.Subscribe<LoginViewModel>(
                this,
                LoginViewModel.LogInFinishedMessage,
                _ => LoadCommand.Execute(null));
        }

        public bool IsNoOneLoggedIn => !AuthenticationService.IsAnyOneLoggedIn;

        public IEnumerable<Tuple<string, string, AsyncCommand>> RecommendedProducts
        {
            get => recommendedProducts;
            set => SetAndRaisePropertyChanged(ref recommendedProducts, value);
        }

        public IEnumerable<ProductViewModel> PopularProducts
        {
            get => popularProducts;
            set => SetAndRaisePropertyChanged(ref popularProducts, value);
        }

        public IEnumerable<ProductDTO> PreviouslySeenProducts
        {
            get => previouslySeenProducts;
            set => SetAndRaisePropertyChanged(ref previouslySeenProducts, value);
        }

        public ICommand PhotoCommand => new AsyncCommand(_ => App.NavigateToAsync(
            new CameraPreviewTakePhotoPage()));

        public ICommand ARCommand => new AsyncCommand(_ => App.NavigateToAsync(new CameraPreviewPage()));

        public ICommand LoadCommand => new AsyncCommand(_ => LoadDataAsync());

        public override async Task InitializeAsync()
        {
            try
            {
                await base.InitializeAsync();

                await AuthenticationService.RefreshSessionAsync();

                if (IsNoOneLoggedIn)
                {
                    await App.NavigateModallyToAsync(new LogInPage());
                    IsBusy = false;
                }
                else
                {
                    await LoadDataAsync();
                    IsBusy = false;
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }

            //TODO: this goes away
            //await LoadDataAsync();
            //IsBusy = false;
        }

        public override async Task UninitializeAsync()
        {
            await base.UninitializeAsync();
        }

        private async Task LoadDataAsync()
        {
            CurrentState = State.EverythingOK;

            RecommendedProducts = new List<Tuple<string, string, AsyncCommand>>
            {
                Tuple.Create("Hardware", "recommended_powertools.jpg",
                    new AsyncCommand(_ =>
                    {
                        Analytics.TrackEvent(AnalyticEvents.ProductCategoryViewEvent,
                            new Dictionary<string,string>{
                                {AnalyticEvents.FromPageAreaKey, AnalyticEvents.RecommendedAreaName },
                                {AnalyticEvents.FromPageEventKey,AnalyticEvents.HomePageName },
                                {AnalyticEvents.ProductCategoryKey, AnalyticEvents.Hardware }
                            });

                        return App.NavigateToAsync(new ProductCategoryPage(DefaultSettings.Hardware), closeFlyout: false);
                    }
                )),
                Tuple.Create("Electrical", "recommended_lighting.jpg",
                    new AsyncCommand(_ =>
                    {
                        Analytics.TrackEvent(AnalyticEvents.ProductCategoryViewEvent,
                            new Dictionary<string,string>{
                                {AnalyticEvents.FromPageAreaKey, AnalyticEvents.RecommendedAreaName },
                                {AnalyticEvents.FromPageEventKey, AnalyticEvents.HomePageName },
                                {AnalyticEvents.ProductCategoryKey, AnalyticEvents.Electrical }
                            });

                        return App.NavigateToAsync(new ProductCategoryPage(DefaultSettings.Electrical), closeFlyout: false);
                    }
                )),
                Tuple.Create("Tiles", "recommended_bathrooms.jpg",
                    new AsyncCommand(_ =>
                    {
                        Analytics.TrackEvent(AnalyticEvents.ProductCategoryViewEvent,
                            new Dictionary<string,string>{
                                {AnalyticEvents.FromPageAreaKey, AnalyticEvents.RecommendedAreaName },
                                {AnalyticEvents.FromPageEventKey,AnalyticEvents.HomePageName },
                                {AnalyticEvents.ProductCategoryKey, AnalyticEvents.Tiles }
                            });

                        return App.NavigateToAsync(new ProductCategoryPage(DefaultSettings.Tiles), closeFlyout: false);
                    }
                )),
                Tuple.Create("Hinges", "recommended_hinges.jpg",
                    new AsyncCommand(_ =>
                    {
                        Analytics.TrackEvent(AnalyticEvents.ProductCategoryViewEvent,
                            new Dictionary<string,string>{
                                {AnalyticEvents.FromPageAreaKey, AnalyticEvents.RecommendedAreaName },
                                {AnalyticEvents.FromPageEventKey, AnalyticEvents.HomePageName },
                                {AnalyticEvents.ProductCategoryKey, AnalyticEvents.Hinges }
                            });

                        return App.NavigateToAsync(new ProductCategoryPage(DefaultSettings.Hinges), closeFlyout: false);
                    }
                ))
            };

            var productSvc = DependencyService.Get<IProductService>();

            var homeResult = await TryExecuteWithLoadingIndicatorsAsync(
                productSvc.GetProductsAsync()
            );

            if (homeResult.IsError || homeResult.Value == null || homeResult.Value.Products == null)
            {
                CurrentState = State.Error;
                return;
            }

            var popularProductsRaw = homeResult.Value.Products;

            var popularProductsWithCommand = popularProductsRaw.Shuffle().Take(3).Select(
                item => new ProductViewModel(item, FeatureNotAvailableCommand));

            PopularProducts = new List<ProductViewModel>(popularProductsWithCommand);

            var randomProducts = popularProductsRaw.Shuffle().Take(3);
            PreviouslySeenProducts = new List<ProductDTO>(randomProducts);
        }
    }
}
