using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace TailwindTraders.Mobile
{
    public partial class CartPage
    {
        public CartPage()
        {
            InitializeComponent();

            BindingContext = new ShoppingCartViewModel();
        }

        internal override IEnumerable<VisualElement> GetStateAwareVisualElements()
        {
            return new VisualElement[] { infoLabel };
        }
    }
}
