using System;
using System.IO;
using System.Threading.Tasks;
using System.Security.Authentication;
using System.Linq;

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
    public static class GetAllProducts
    {
        static MongoClient mongoClient;
        static IMongoCollection<ProductDTO> productsCollection;

        static IMongoCollection<ProductDTO> ProductsCollection
        {
            get 
            {
                if (productsCollection == null)
                {
                    var settings = MongoClientSettings.FromUrl(
                        new MongoUrl(Environment.GetEnvironmentVariable("mongoConnectionString"))
                    );
                    settings.SslSettings = new SslSettings { EnabledSslProtocols = SslProtocols.Tls12 };

                    mongoClient = new MongoClient(settings);
                                     
                    // This will create or get the database
                    var db = mongoClient.GetDatabase(Environment.GetEnvironmentVariable("inventoryDatabaseName"));

                    // This will create or get the collection
                    var collectionSettings = new MongoCollectionSettings { ReadPreference = ReadPreference.Nearest };
                    productsCollection = db.GetCollection<ProductDTO>(
                        Environment.GetEnvironmentVariable("inventoryCollectionName"), 
                        collectionSettings
                    );
                }
                return productsCollection;
            }
        }

        [FunctionName("GetAllProducts")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "products")] HttpRequest req,
            ILogger log)
        {
            var allProducts = await ProductsCollection.AsQueryable().ToListAsync();

            var returnList = new ProductsPerTypeDTO();
            returnList.Products = allProducts;
            returnList.Size = allProducts.Count();

            return new OkObjectResult(returnList);
        }        
    }
}
