using TailwindTraders.Mobile.Features.Home;
using TailwindTraders.Mobile.Features.LogIn;
using TailwindTraders.Mobile.Features.Product;

namespace TailwindTraders.Mobile.Features.Common
{
    public class FakeRestPoolService : IRestPoolService
    {
        public IProfilesAPI ProfilesAPI { get; } = new FakeProfilesAPI();

        public IHomeAPI HomeAPI { get; } = new FakeHomeAPI();

        public IProductsWebAPI ProductsAPI { get; } = null;  //new FakeProductsAPI();

        public IShoppingCartAPI ShoppingCartAPI { get; } = null;

        public ILoginAPI LoginAPI { get; } = new FakeLoginAPI();

        public ISimilarProductsAPI SimilarProductsAPI { get; } = new FakeSimilarProductsAPI();

        public void UpdateApiUrl(string newApiUrl)
        {
            // Intentionally blank
        }
    }
}
