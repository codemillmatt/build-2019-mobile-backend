using System.Collections.Generic;
using System.Threading.Tasks;

using ShoppingCartService.api.Models;

namespace ShoppingCartService.api.DataService
{
    public interface IMongoService
    {
        Task<IEnumerable<ShoppingCartItem>> GetCartItemsForUser(string userId);
        Task AddCartItem(ShoppingCartItem item);
        Task RemoveCartItem(long id, string userId);
        Task RemoveAllCartItemsForUser(string userId);
    }
}