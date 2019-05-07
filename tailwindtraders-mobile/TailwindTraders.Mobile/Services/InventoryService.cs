using System;
using System.Threading.Tasks;

using TailwindTraders.Mobile;
using Xamarin.Forms;
using Refit;
using TailwindTraders.Mobile.Features.Settings;
using TailwindTraders.Mobile.Helpers;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AppCenter.Crashes;

[assembly: Dependency(typeof(InventoryService))]
namespace TailwindTraders.Mobile
{
    public class InventoryService : IInventoryService
    {
        IInventoryWebAPI webApi;

        
        string url = "** YOUR INVENTORY URL HERE - up through the api route portion - refit in IInventoryWebAPI appends the rest **";
        // string url = "http://localhost:5000/api";

        public InventoryService()
        {
            webApi = RestService.For<IInventoryWebAPI>(
                HttpClientFactory.Create(url));
        }

        public async Task<long> DecrementInventory(string sku)
        {
            try
            {
                var allInventoryInfo = await webApi.DecrementInventory(sku);

                return allInventoryInfo.FirstOrDefault().Quantity;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return 0;
            }
        }

        public async Task<long> GetCurrentInventory(string sku)
        {
            try
            {
                var allInventoryInfo = await webApi.GetInventoryForSku(sku);

                return allInventoryInfo.First(ii => ii.Sku.Equals(sku)).Quantity;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return 0;
            }
        }

        public async Task<long> IncrementInventory(string sku)
        {
            try
            {
                var allInventoryInfo = await webApi.IncrementInventory(sku);

                return allInventoryInfo.FirstOrDefault().Quantity;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return 0;
            }
        }
    }

    public interface IInventoryWebAPI
    {
        [Get("/inventory?skus={sku}")]
        Task<List<InventoryInfo>> GetInventoryForSku(string sku);

        [Post("/inventory/{sku}/increment")]
        Task<List<InventoryInfo>> IncrementInventory(string sku);

        [Post("/inventory/{sku}/decrement")]
        Task<List<InventoryInfo>> DecrementInventory(string sku);
    }
}
