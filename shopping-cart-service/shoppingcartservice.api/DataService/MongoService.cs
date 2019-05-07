using System.Collections.Generic;
using System;
using System.Security.Authentication;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;

using ShoppingCartService.api.Models;

using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Driver.Linq;

namespace ShoppingCartService.api.DataService
{
    public class MongoService : IMongoService
    {
        string databaseName = string.Empty;
        string collectionName = string.Empty;
        string connectionString = string.Empty;

        IMongoCollection<ShoppingCartItem> cartItemCollection;
        IMongoCollection<ShoppingCartItem> CartItemCollection
        {
            get
            {
                if (cartItemCollection == null)
                {
                    // APIKeys.Connection string is found in the portal under the "Connection String" blade
                    MongoClientSettings settings = MongoClientSettings.FromUrl(
                      new MongoUrl(connectionString)
                    );

                    settings.SslSettings =
                        new SslSettings() { EnabledSslProtocols = SslProtocols.Tls12 };

                    // Initialize the client
                    var mongoClient = new MongoClient(settings);                    

                    // This will create or get the database
                    var db = mongoClient.GetDatabase(databaseName);

                    // This will create or get the collection
                    var collectionSettings = new MongoCollectionSettings { ReadPreference = ReadPreference.Nearest };
                    cartItemCollection = db.GetCollection<ShoppingCartItem>(collectionName, collectionSettings);
                }
                return cartItemCollection;
            }
        }

        public MongoService(IConfiguration config)
        {
            //TODO: put these into dotnet user-secrets set 'databaseName' 'XXX' and then pull out config["databaseName"]
            // databaseName = config.GetSection("MongoConnectionInfo:databaseName").Value;
            // collectionName = config.GetSection("MongoConnectionInfo:collectionName").Value;

            databaseName = config["databaseName"];
            collectionName = config["collectionName"];
            connectionString = config["connectionString"];            
        }

        public async Task<IEnumerable<ShoppingCartItem>> GetCartItemsForUser(string userId)
        {
            var allItems = await CartItemCollection
                    .Find(item => item.UserId == userId)
                    .ToListAsync();

            return allItems;
        }

        public async Task AddCartItem(ShoppingCartItem item)
        {
            item.DateModified = DateTime.UtcNow;

            await CartItemCollection.InsertOneAsync(item);
        }

        public async Task RemoveCartItem(long id, string userId)
        {            
            await CartItemCollection.DeleteOneAsync(t => t.ProductId.Equals(id) && t.UserId.Equals(userId));                        
        }     

        public async Task RemoveAllCartItemsForUser(string userId)
        {
            await CartItemCollection.DeleteManyAsync(t => t.UserId.Equals(userId));           
        }   
    }
}