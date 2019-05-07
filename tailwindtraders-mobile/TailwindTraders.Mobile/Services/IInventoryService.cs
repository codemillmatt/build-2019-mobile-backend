using System;
using System.Threading.Tasks;

namespace TailwindTraders.Mobile
{
    public interface IInventoryService
    {
        Task<long> GetCurrentInventory(string sku);
        Task<long> IncrementInventory(string sku);
        Task<long> DecrementInventory(string sku);
    }
}
