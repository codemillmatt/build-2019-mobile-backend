using System;
using System.IO;
using System.Threading.Tasks;
using System.Security.Authentication;
using System.Linq;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Driver.Linq;

namespace TailwindTraders.Functions
{
    public static class ShoppingCartFunctions
    {
        static MongoClient mongoClient;
        static IMongoCollection<ShoppingCartItem> cartItemsCollection;

        static IMongoCollection<ShoppingCartItem> CartItemsCollection
        {
            get 
            {
                if (cartItemsCollection == null)
                {
                    var settings = MongoClientSettings.FromUrl(
                        new MongoUrl(Environment.GetEnvironmentVariable("mongoConnectionString"))
                    );
                    settings.SslSettings = new SslSettings { EnabledSslProtocols = SslProtocols.Tls12 };

                    mongoClient = new MongoClient(settings);
                                     
                    // This will create or get the database
                    var db = mongoClient.GetDatabase(Environment.GetEnvironmentVariable("cartDatabaseName"));

                    // This will create or get the collection
                    var collectionSettings = new MongoCollectionSettings { ReadPreference = ReadPreference.Nearest };
                    cartItemsCollection = db.GetCollection<ShoppingCartItem>(
                        Environment.GetEnvironmentVariable("cartCollectionName"), 
                        collectionSettings
                    );
                }
                return cartItemsCollection;
            }
        }
        
        [FunctionName("GetShoppingCartItemsForUser")]
        public static async Task<IActionResult> RunGetShoppingCartItemsForUser (
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "shoppingcart/{userName}")] HttpRequest req,
            string userName,
            ILogger log)
        {
            var cartItemsForUser = await CartItemsCollection
                .Find(ci => ci.UserId == userName)
                .ToListAsync();

            return new OkObjectResult(cartItemsForUser);
        }
    
        [FunctionName("AddItemToCart")]
        public static async Task<IActionResult> RunAddItemToCart (
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route="shoppingcart/{userName}")] HttpRequest req,
            string userName,
            ILogger log)
        {
            var body = await new StreamReader(req.Body).ReadToEndAsync();
            var prodInfo = JsonConvert.DeserializeObject<BareProductInfo>(body);
            
            var cartItem = new ShoppingCartItem { 
                DateModified = DateTime.UtcNow,
                UserId = userName,
                ProductId = prodInfo.ProductId,
                Sku = prodInfo.Sku,
                Quantity = 1
            };
            
            await CartItemsCollection.InsertOneAsync(cartItem);

            return new OkResult();
        }
        
        [FunctionName("DeleteItemFromCart")]
        public static async Task<IActionResult> RunDeleteItemFromCart (
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route="shoppingcart/{userName}")] HttpRequest req,
            string userName,
            ILogger log)
        {
            var body = await new StreamReader(req.Body).ReadToEndAsync();
            var prodInfo = JsonConvert.DeserializeObject<BareProductInfo>(body);
            
            await CartItemsCollection.DeleteOneAsync(ci => ci.ProductId.Equals(prodInfo.ProductId)
                && ci.UserId.Equals(userName));

            return new OkResult();
        }

        // USE THIS AS VERSION 1 - change the name to be the same as below

        [FunctionName("DeleteAllItems")]
        public static async Task<IActionResult> RunDeleteAllItems (
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route="shoppingcart/removeall/{userName}")] HttpRequest req,
            string userName,
            ILogger log)
        {
            await CartItemsCollection.DeleteManyAsync(ci => ci.UserId == userName);

            return new OkResult();
        }


        // USE THIS AS VERSION 2
        [FunctionName("PurchaseItems")]
        public static async Task<IActionResult> RunPurchaseItems (
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route="shoppingcart/{userName}/purchase")] HttpRequest req,            
            string userName,
            ILogger log)
        {            
                
            // get the products to purchase out of the request
            var body = await new StreamReader(req.Body).ReadToEndAsync();
            var prodsToPurchase = JsonConvert.DeserializeObject<List<BareProductInfo>>(body);

            // get the products stored in the shopping cart
            var shoppingCartProducts = await CartItemsCollection
                .Find(ci => ci.UserId == userName)
                .ToListAsync();

            // make sure everything in the cart is in the incoming request
            var notInProdsToPurchase = shoppingCartProducts.Count(sci => !prodsToPurchase.Any(ptp => ptp.ProductId == sci.ProductId));
            var notInCart = prodsToPurchase.Count(ptp => !shoppingCartProducts.Any(sci => sci.ProductId == ptp.ProductId));
            
            var returnInfo = ValueTuple.Create(shoppingCartProducts, prodsToPurchase);
            
            if (notInProdsToPurchase > 0 || notInCart > 0)
                return new ConflictObjectResult(returnInfo);

            await CartItemsCollection.DeleteManyAsync(ci => ci.UserId == userName);

            return new OkObjectResult(returnInfo);
        }
    }
}
