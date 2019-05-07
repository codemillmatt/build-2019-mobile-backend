using System;
using TailwindTraders.Mobile.Features.Product.Detail;
namespace TailwindTraders.Mobile.Features.Product.Category
{
    public partial class ProductView
    {
        async void Handle_Tapped(object sender, System.EventArgs e)
        {
            var vm = BindingContext as ProductViewModel;

            await App.NavigateToAsync(new ProductDetailPage(vm.Product.ItemId));


        }

        public ProductView()
        {
            InitializeComponent();
        }
    }
}
