using System;
using TailwindTraders.Mobile.Framework;
using TailwindTraders.Mobile.Features.Product;
using System.Collections.Generic;
using System.Threading.Tasks;
using TailwindTraders.Mobile.Helpers;
using System.Linq;
using Xamarin.Forms;

using System.Net;
using System.Net.Http;
using Microsoft.AppCenter.Crashes;
using TailwindTraders.Mobile.Features.LogIn;
using OperationResult.Tags;
using Newtonsoft.Json;

using TailwindTraders.Mobile.Features.Settings;

namespace TailwindTraders.Mobile
{
    public class ShoppingCartViewModel : BaseStateAwareViewModel<ShoppingCartViewModel.State>
    {
        readonly IAuthenticationService authService;
        readonly IProductService productService;
        readonly IShoppingCartService shoppingCartService;

        public ShoppingCartViewModel()
        {
            Title = "Your Cart";

            authService = DependencyService.Get<IAuthenticationService>();
            productService = DependencyService.Get<IProductService>();
            shoppingCartService = DependencyService.Get<IShoppingCartService>();

            MessagingCenter.Subscribe<ShoppingCartMessages>(this, ShoppingCartMessages.RemoveItemFromCart,
                async (obj) =>
                {
                    var bareItemInfo = new BareShoppingCartItemInfo
                    {
                        ProductId = $"{obj.ProductId}",
                        Sku = $"TT{obj.ProductId}"
                    };

                    await shoppingCartService.DeleteItemFromCart(
                        await authService.GetUserName(),
                        bareItemInfo
                    );

                    await LoadDataAsync();
                }
            );
        }

        public IEnumerable<ProductDTO> itemsInCart;
        public IEnumerable<ProductDTO> ItemsInCart
        {
            get => itemsInCart;
            set => SetAndRaisePropertyChanged(ref itemsInCart, value);
        }

        string title;
        public string Title
        {
            get => title;
            set => SetAndRaisePropertyChanged(ref title, value);
        }

        public enum State
        {
            EverythingOK,
            Error,
            Empty
        }

        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            List<ProductDTO> shoppingItemProducts = new List<ProductDTO>();

            CurrentState = State.EverythingOK;

            // TODO: write a blog on booting the .net core service up with the --url flag
            // https://www.osstatus.com/search/results?platform=all&framework=all&search=9807
            // was getting a bunch of random errors with both http & https were enabled.

            try
            {
                var userName = await authService.GetUserName();

                List<ShoppingCartItem> shoppingCartItems = new List<ShoppingCartItem>();

                try
                {
                    shoppingCartItems = await shoppingCartService.GetCartItemsForUser(userName).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                foreach (var item in shoppingCartItems)
                {
                    long.TryParse(item.ProductId, out long prodId);

                    var tempProdItem = await productService.GetProductDetailAsync(prodId).ConfigureAwait(false);

                    shoppingItemProducts.Add(tempProdItem);
                }

                ItemsInCart = shoppingItemProducts;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public Command PurchaseItemsCommand => new AsyncCommand(async () =>
        {
            try
            {
                var userName = await authService.GetUserName();

                var success = await shoppingCartService.PurchaseCart(userName);

                if (success)
                    await App.NavigateModallyBackAsync();
                else
                    await XSnackService.ShowMessageAsync("Oh no! Something went wrong!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

            }
        });

        public Command ContinueShoppingCommand => new AsyncCommand(async () =>
            await App.NavigateModallyBackAsync());
    }
}