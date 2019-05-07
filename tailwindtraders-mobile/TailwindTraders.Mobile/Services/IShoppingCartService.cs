using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TailwindTraders.Mobile
{
    public interface IShoppingCartService
    {
        Task<List<ShoppingCartItem>> GetCartItemsForUser(string userId);
        Task AddItemToCart(string userId, BareShoppingCartItemInfo item);
        Task DeleteItemFromCart(string userId, BareShoppingCartItemInfo item);
        Task<bool> PurchaseCart(string userId);
    }
}
