using System.Collections.Generic;
using Xamarin.Forms;

namespace TailwindTraders.Mobile.Features.Product.Detail
{
    public partial class ProductDetailPage
    {
        public ProductDetailPage(long productId)
        {
            InitializeComponent();

            BindingContext = new ProductDetailViewModel(productId);
        }

        internal override IEnumerable<VisualElement> GetStateAwareVisualElements() => new VisualElement[]
        {
            refreshButton,
            stateAwareStackLayout,
        };
    }
}
