using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TailwindTraders.Mobile.Features.Product.Detail;
using TailwindTraders.Mobile.Framework;
using Xamarin.Forms;
using System;

namespace TailwindTraders.Mobile.Features.Product.Category
{
    public class ProductCategoryViewModel : BaseStateAwareViewModel<ProductCategoryViewModel.State>
    {
        private readonly string typeId;

        public enum State
        {
            EverythingOK,
            Error,
            Empty,
        }

        private bool isAlreadyInitialized;
        private string title;
        private IEnumerable<ProductDTO> products;
        private IProductService productService;

        public string Title
        {
            get => title;
            set => SetAndRaisePropertyChanged(ref title, value);
        }

        public IEnumerable<ProductDTO> Products
        {
            get => products;
            set => SetAndRaisePropertyChanged(ref products, value);
        }

        public ICommand LoadCommand { get; }

        public ICommand DetailCommand { get; }

        public ProductCategoryViewModel(string typeId)
        {
            this.typeId = typeId;

            LoadCommand = new AsyncCommand(() => LoadDataAsync(typeId));
            DetailCommand = new Command<int>(
                productId =>
                {
                    Console.WriteLine("hi");
                    App.NavigateToAsync(new ProductDetailPage(productId)).ConfigureAwait(false);
                }
            );

            productService = DependencyService.Get<IProductService>();
        }

        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            if (isAlreadyInitialized)
            {
                return;
            }

            isAlreadyInitialized = true;
            LoadCommand.Execute(null);
        }

        private async Task LoadDataAsync(string category)
        {
            CurrentState = State.EverythingOK;
            Products = null;

            var response = await TryExecuteWithLoadingIndicatorsAsync(
                productService.GetProductsPerCategory(category)
            );

            if (response.IsError)
            {
                CurrentState = State.Error;
                return;
            }

            if (response.Value == null || response.Value.Products == null || !response.Value.Products.Any())
            {
                CurrentState = State.Empty;
                return;
            }

            Products = response.Value.Products;

            Title = products.First().ProductType.ToString();
        }
    }
}
