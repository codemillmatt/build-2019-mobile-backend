using System.Collections.Generic;
using System.Windows.Input;
using Microsoft.AppCenter.Analytics;

namespace TailwindTraders.Mobile.Features.Product
{
    public class ProductViewModel
    {
        public ProductViewModel(ProductDTO product, ICommand command)
        {
            Analytics.TrackEvent(AnalyticEvents.ProductDetailViewEvent,
                new Dictionary<string, string>{
                    {AnalyticEvents.FromPageAreaKey, AnalyticEvents.NotApplicableAreaName },
                    {AnalyticEvents.FromPageEventKey,AnalyticEvents.ProductDetailPageName },
                    {AnalyticEvents.ProductCategoryKey, product.Sku }
                });

            Product = product;
            Command = command;
        }

        public ProductDTO Product { get; }

        public ICommand Command { get; }
    }
}
