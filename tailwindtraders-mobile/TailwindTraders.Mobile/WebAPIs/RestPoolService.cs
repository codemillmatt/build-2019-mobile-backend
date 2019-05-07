using Refit;
using TailwindTraders.Mobile.Features.Home;
using TailwindTraders.Mobile.Features.LogIn;
using TailwindTraders.Mobile.Features.Product;
using TailwindTraders.Mobile.Features.Settings;
using TailwindTraders.Mobile.Helpers;
using TailwindTraders.Mobile;

namespace TailwindTraders.Mobile.Features.Common
{
    public class RestPoolService : IRestPoolService
    {
        public IProfilesAPI ProfilesAPI { get; private set; }

        public IHomeAPI HomeAPI { get; private set; }

        public ILoginAPI LoginAPI { get; private set; }

        public ISimilarProductsAPI SimilarProductsAPI { get; private set; }

        public RestPoolService()
        {
            UpdateApiUrl(DefaultSettings.RootApiUrl);

            SimilarProductsAPI = RestService.For<ISimilarProductsAPI>(
                HttpClientFactory.Create(DefaultSettings.RootProductsWebApiUrl));
        }

        public void UpdateApiUrl(string newApiUrl)
        {
            // TODO: As you go through and implement the various APIs, update these from the fake to the real
            ProfilesAPI = new FakeProfilesAPI();
            HomeAPI = new FakeHomeAPI();
            LoginAPI = new FakeLoginAPI();


            //ProfilesAPI = RestService.For<IProfilesAPI>(HttpClientFactory.Create(newApiUrl));
            //HomeAPI = RestService.For<IHomeAPI>(HttpClientFactory.Create(newApiUrl));

            //LoginAPI = RestService.For<ILoginAPI>(HttpClientFactory.Create(newApiUrl));
        }
    }
}
