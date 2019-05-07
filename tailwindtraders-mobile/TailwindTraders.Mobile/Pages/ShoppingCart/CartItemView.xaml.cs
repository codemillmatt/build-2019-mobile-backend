using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace TailwindTraders.Mobile
{
    public partial class CartItemView : ContentView
    {
        public CartItemView()
        {
            InitializeComponent();
        }

        public static readonly BindableProperty ProductIdProperty =
            BindableProperty.Create("ProductId", typeof(long), typeof(CartItemView));

        public long ProductId
        {
            get { return (long)GetValue(ProductIdProperty); }
            set { SetValue(ProductIdProperty, value); }
        }

        void Handle_Tapped(object sender, System.EventArgs e)
        {
            var msg = new ShoppingCartMessages { ProductId = this.ProductId };
            MessagingCenter.Send(msg, ShoppingCartMessages.RemoveItemFromCart);
        }
    }
}
