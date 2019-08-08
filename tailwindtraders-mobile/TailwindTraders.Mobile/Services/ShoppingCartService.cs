using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TailwindTraders.Mobile;
using Xamarin.Forms;

using TailwindTraders.Mobile.Framework;
using TailwindTraders.Mobile.Features.Common;
using MonkeyCache.SQLite;
using MonkeyCache;
using Refit;
using TailwindTraders.Mobile.Helpers;
using TailwindTraders.Mobile.Features.Product;
using System.Linq;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter.Analytics;

[assembly: Dependency(typeof(ShoppingCartService))]
namespace TailwindTraders.Mobile
{
    public class ShoppingCartService : IShoppingCartService
    {
        readonly IConnectivityService connectivityService;
        readonly IShoppingCartAPI webApi;

        //string url = "** YOUR SHOPPING CART URL HERE - up through the api route portion - refit in IShoppingCartAPI appends the rest **";
        //readonly string shoppingCartApiUrl = "http://localhost:9000/api";
        readonly string shoppingCartApiUrl = "https://bema-shoppingcart.azurewebsites.net/api";

        readonly string cartItemsForUserKey = "cartItems";

        public ShoppingCartService()
        {
            connectivityService = DependencyService.Get<IConnectivityService>();
            webApi = RestService.For<IShoppingCartAPI>(
                HttpClientFactory.Create(shoppingCartApiUrl)
            );
        }

        public async Task<List<ShoppingCartItem>> GetCartItemsForUser(string userId)
        {
            var currentCartKey = $"{cartItemsForUserKey}-{userId}";

            if (!connectivityService.IsThereInternet && Barrel.Current.Exists(currentCartKey))
            {
                return Barrel.Current.Get<List<ShoppingCartItem>>(currentCartKey);
            }

            if (!Barrel.Current.IsExpired(currentCartKey) && Barrel.Current.Exists(currentCartKey))
            {
                return Barrel.Current.Get<List<ShoppingCartItem>>(currentCartKey);
            }

            var cartItemsForUser = await webApi.GetItemsForUserAsync(userId).ConfigureAwait(false);

            Barrel.Current.Add(currentCartKey, cartItemsForUser, TimeSpan.FromSeconds(60));

            return cartItemsForUser;
        }

        // intentional bug where we don't check send anything up that's been added while offline and send those items up to the server when back online
        public async Task AddItemToCart(string userId, BareShoppingCartItemInfo item)
        {
            var currentCartKey = $"{cartItemsForUserKey}-{userId}";

            var currentItems = Barrel.Current.Get<List<ShoppingCartItem>>(currentCartKey) ?? new List<ShoppingCartItem>();

            currentItems.Add(new ShoppingCartItem() { ProductId = item.ProductId, Sku = item.Sku, Quantity = 1, UserId = userId, DateModified = DateTime.Now });

            Barrel.Current.Add(currentCartKey, currentItems, TimeSpan.FromMinutes(30));

            if (connectivityService.IsThereInternet)
            {
                await webApi.AddItemToCart(userId, item).ConfigureAwait(false);
            }
        }

        // intentional offline bug - we don't check for anything that's been deleted while offline and then send up those deleted items to the server when back online
        public async Task DeleteItemFromCart(string userId, BareShoppingCartItemInfo item)
        {
            var currentCartKey = $"{cartItemsForUserKey}-{userId}";

            var currentItems = Barrel.Current.Get<List<ShoppingCartItem>>(currentCartKey);
            currentItems.Remove(currentItems.FirstOrDefault(ci => ci.ProductId == item.ProductId));

            Barrel.Current.Add(currentCartKey, currentItems, TimeSpan.FromMinutes(30));

            if (connectivityService.IsThereInternet)
            {
                await webApi.DeleteItemFromCart(userId, item).ConfigureAwait(false);
            }
        }
        
        public async Task<bool> PurchaseCart(string userId)
        {
            var currentCartKey = $"{cartItemsForUserKey}-{userId}";

            if (connectivityService.IsThereInternet)
            {
                IInventoryService inventoryService = DependencyService.Get<IInventoryService>();

                // Get current items from on-device cache
                var currentItems = Barrel.Current.Get<List<ShoppingCartItem>>(currentCartKey);

                List<BareShoppingCartItemInfo> currentCart = new List<BareShoppingCartItemInfo>();
                foreach (var product in currentItems)
                {
                    var bareInfo = new BareShoppingCartItemInfo { ProductId = product.ProductId, Sku = product.Sku };
                    currentCart.Add(bareInfo);
                }

                try
                {
                    // send the on-device items to cloud to purchase. May result in a conflict if cloud stored items don't match
                    await webApi.PurchaseItems(userId, currentCart).ConfigureAwait(false);

                    Barrel.Current.Empty(currentCartKey);
                }
                catch (ApiException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    // We have a conflict. The ApiException object will have more info in it as to the objects that don't match
                    Analytics.TrackEvent("conflict during purchase");

                    return false;
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                    return false;
                }

                // Decrement the inventory for everything
                foreach (var product in currentItems)
                {
                    await inventoryService.DecrementInventory(product.Sku);
                }
            }
            else
            {
                return false;
            }

            return true;
        }
    }

    public interface IShoppingCartAPI
    {
        [Get("/shoppingcart/{userId}")]
        Task<List<ShoppingCartItem>> GetItemsForUserAsync(string userId);

        [Get("/shoppingcart")]
        Task<ProductDTO> GetSingleItem();

        [Post("/shoppingcart/{userId}")]
        Task AddItemToCart(string userId, [Body(BodySerializationMethod.Json)]BareShoppingCartItemInfo itemInfo);

        [Delete("/shoppingcart/{userId}")]
        Task DeleteItemFromCart(string userId, [Body(BodySerializationMethod.Json)]BareShoppingCartItemInfo itemInfo);

        // VERSION 2
        [Post("/shoppingcart/{userId}/purchase")]
        Task PurchaseItems(string userId, [Body(BodySerializationMethod.Json)]List<BareShoppingCartItemInfo> shoppingCartItems);
    }
}
