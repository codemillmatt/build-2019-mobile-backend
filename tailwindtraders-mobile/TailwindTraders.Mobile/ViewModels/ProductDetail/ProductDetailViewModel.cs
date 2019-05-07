using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TailwindTraders.Mobile.Framework;
using TailwindTraders.Mobile.Helpers;
using Xamarin.Forms;
using TailwindTraders.Mobile.Features.LogIn;
using System;

namespace TailwindTraders.Mobile.Features.Product.Detail
{
    public class ProductDetailViewModel : BaseStateAwareViewModel<ProductDetailViewModel.State>
    {
        private readonly long productId;

        private string title;
        private IEnumerable<string> pictures;
        private string brand;
        private string name;
        private string price;
        private long inventoryQuantity;
        private IEnumerable<FeatureDTO> features;
        private IEnumerable<ProductViewModel> similarProducts;
        private IEnumerable<ProductDTO> alsoBoughtProducts;

        readonly private IAuthenticationService authService;
        readonly private IProductService productService;
        readonly private IShoppingCartService shoppingCartService;
        readonly private IInventoryService inventoryService;

        #region Properties 

        public enum State
        {
            EverythingOK,
            Error,
        }

        public string Title
        {
            get => title;
            set => SetAndRaisePropertyChanged(ref title, value);
        }

        public IEnumerable<string> Pictures
        {
            get => pictures;
            set => SetAndRaisePropertyChanged(ref pictures, value);
        }

        public string Brand
        {
            get => brand;
            set => SetAndRaisePropertyChanged(ref brand, value);
        }

        public string Name
        {
            get => name;
            set => SetAndRaisePropertyChanged(ref name, value);
        }

        public string Price
        {
            get => price;
            set => SetAndRaisePropertyChanged(ref price, value);
        }

        public long InventoryQuantity
        {
            get => inventoryQuantity;
            set => SetAndRaisePropertyChanged(ref inventoryQuantity, value);
        }

        public IEnumerable<FeatureDTO> Features
        {
            get => features;
            set => SetAndRaisePropertyChanged(ref features, value);
        }

        public IEnumerable<ProductViewModel> SimilarProducts
        {
            get => similarProducts;
            set => SetAndRaisePropertyChanged(ref similarProducts, value);
        }

        public IEnumerable<ProductDTO> AlsoBoughtProducts
        {
            get => alsoBoughtProducts;
            set => SetAndRaisePropertyChanged(ref alsoBoughtProducts, value);
        }

        public ICommand RefreshCommand { get; }

        #endregion

        public ProductDetailViewModel(long productId)
        {
            this.productId = productId;
            authService = DependencyService.Get<IAuthenticationService>();
            productService = DependencyService.Get<IProductService>();
            shoppingCartService = DependencyService.Get<IShoppingCartService>();
            inventoryService = DependencyService.Get<IInventoryService>();

            RefreshCommand = new Command(async () => await LoadDataAsync());

            AddToShoppingCartCommand = new AsyncCommand(async () =>
            {
                try
                {
                    var userName = await authService.GetUserName().ConfigureAwait(false);

                    await shoppingCartService.AddItemToCart(userName,
                         new BareShoppingCartItemInfo
                         {
                             Sku = $"TT{productId}",
                             ProductId = productId.ToString()
                         }
                     );

                    await XSnackService.ShowMessageAsync("Item added to cart");
                }
                catch (Exception ex)
                {
                    await XSnackService.ShowMessageAsync("Error adding item to cart");
                    Console.WriteLine(ex);
                }
            });
        }

        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            await LoadDataAsync().ConfigureAwait(false);
        }

        private async Task LoadDataAsync()
        {
            await TryExecuteWithLoadingIndicatorsAsync(RequestProductDetailAsync());
        }

        private async Task RequestProductDetailAsync()
        {
            var product = await productService.GetProductDetailAsync(productId);

            if (product != null)
            {
                var qtyTask = inventoryService.GetCurrentInventory(product.Sku);
                var productsPerCategoryTask = productService.GetProductsPerCategory(product.ProductType.ToString());
                var alsoBoughtProductsTask = productService.GetProductsAsync();

                await Task.WhenAll(
                    qtyTask, productsPerCategoryTask, alsoBoughtProductsTask
                );

                var qty = await qtyTask;
                var theSimilarProducts = await productsPerCategoryTask;
                var alsoBoughtProds = await alsoBoughtProductsTask;

                SimilarProducts = theSimilarProducts.Products.Shuffle().Take(5)
                    .Select(item => new ProductViewModel(item, FeatureNotAvailableCommand));

                AlsoBoughtProducts = alsoBoughtProds.Products.Shuffle().Take(3);

                UpdateProduct(product, qty);
            }
        }

        private void UpdateProduct(ProductDTO product, long qty)
        {
            var brandName = product.SupplierName.ToString();
            var productName = product.Name;
            Title = $"{brandName}. {productName}";
            Pictures = new List<string> { product.ImageUrl };
            Brand = brandName;
            Name = productName;
            Price = $"${product.Price}";
            InventoryQuantity = qty;

            // TODO: Fix this
            Features = new List<FeatureDTO>();
        }

        public ICommand AddToShoppingCartCommand { get; }

    }
}
