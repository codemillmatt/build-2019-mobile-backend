using System;
using System.IO;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using Microsoft.WindowsAzure.Storage.Queue;

using Newtonsoft.Json;

namespace TailwindTraders.Functions
{    
    public static class InventoryFunctions
    {
        [FunctionName("Inventory")]
        public static async Task<IActionResult> RunGetInventoryForSKU(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "inventory")] HttpRequest req,
            ILogger log)
        {
            try
            {
                List<ItemInventoryType> inventoryInfo = new List<ItemInventoryType>();

                // going to assume only 1 sku at a time
                string sku = req.Query["skus"];
                string sqlConnectionString = Environment.GetEnvironmentVariable("sqlConnectionString");

                using (SqlConnection conn = new SqlConnection(sqlConnectionString))
                {
                    await conn.OpenAsync();

                    var sql = $"SELECT Sku, Quantity, Modified FROM Inventory WHERE Sku = '{sku}'";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        var reader = await cmd.ExecuteReaderAsync();
                        
                        if (reader.HasRows)
                        {
                            if (await reader.ReadAsync())
                            {
                                var inventoryItem = new ItemInventoryType
                                {
                                    Sku = reader.GetString(0),
                                    Quantity = reader.GetInt32(1),
                                    Modified = reader.GetDateTime(2)
                                };

                                inventoryInfo.Add(inventoryItem);

                                reader.Close();
                            }
                        }
                        else 
                        {
                            reader.Close();

                            // Just add a dummy one for next time
                            string insertSQL = $"INSERT INTO Inventory (Sku, Quantity, Modified) VALUES ('{sku}', 12, GETDATE())";
                            cmd.CommandText = insertSQL;
                            await cmd.ExecuteNonQueryAsync();

                            var inventoryItem = new ItemInventoryType
                            {
                                Sku = sku,
                                Quantity = 12,
                                Modified = DateTime.Now
                            };

                            inventoryInfo.Add(inventoryItem);
                        }
                    }
                }
                
                return new OkObjectResult(inventoryInfo);
            } 
            catch (Exception ex)
            {
                log.LogError(ex, "Inventory Finder");
                return new StatusCodeResult(500);
            }
        }

        [FunctionName("InventoryIncrement")]        
        public async static Task<IActionResult> RunIncrementInventory(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "inventory/{sku}/increment")] HttpRequest req,
            [Queue("inventory-change",Connection="AzureWebJobsStorage")]IAsyncCollector<InventoryChangeQueueInfo> changeInfo,
            string sku,
            ILogger log)
        {
            // add to the queue
            await changeInfo.AddAsync(new InventoryChangeQueueInfo { Sku = sku, Operation = "increment" });
             
            // perform the increment
            List<ItemInventoryType> inventoryInfo = await Increment(sku);
            
            return new OkObjectResult(inventoryInfo);                      
        }

        [FunctionName("InventoryDecrement")]        
        public async static Task<IActionResult> RunDecrementInventory(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "inventory/{sku}/decrement")] HttpRequest req,
            [Queue("inventory-change",Connection="AzureWebJobsStorage")]IAsyncCollector<InventoryChangeQueueInfo> changeInfo,
            string sku,
            ILogger log)
        {
            // add to the queue
            await changeInfo.AddAsync(new InventoryChangeQueueInfo { Sku = sku, Operation = "decrement" });

            // perform the decrement
            List<ItemInventoryType> inventoryInfo = await Decrement(sku);
            
            return new OkObjectResult(inventoryInfo);
        }

        [FunctionName("HandleInventoryQueue")]
        public static void HandleInventory(
            [QueueTrigger("inventory-change", Connection="AzureWebJobsStorage")]InventoryChangeQueueInfo queueInfo,
            ILogger log)
        {
           // do something cool here
           log.Log(LogLevel.Information, queueInfo.Sku);
        }
    
        static async Task<List<ItemInventoryType>> Increment(string sku)
        {
            List<ItemInventoryType> inventoryInfo = new List<ItemInventoryType>();

            try
            {             
                string updateSql = $"UPDATE Inventory SET Quantity = i.Quantity + 1 FROM Inventory as i where i.sku = '{sku}'";
                var selectSql = $"SELECT Sku, Quantity, Modified FROM Inventory WHERE Sku = '{sku}'";

                string sqlConnectionString = Environment.GetEnvironmentVariable("sqlConnectionString");

                using (SqlConnection conn = new SqlConnection(sqlConnectionString))
                {
                    await conn.OpenAsync();
                    
                    using (SqlCommand cmd = new SqlCommand(updateSql, conn))
                    {
                        await cmd.ExecuteNonQueryAsync();    

                        cmd.CommandText = selectSql;
                        var reader = await cmd.ExecuteReaderAsync();
                        
                        if (reader.HasRows)
                        {
                            if (await reader.ReadAsync())
                            {
                                var inventoryItem = new ItemInventoryType
                                {
                                    Sku = reader.GetString(0),
                                    Quantity = reader.GetInt32(1),
                                    Modified = reader.GetDateTime(2)
                                };

                                inventoryInfo.Add(inventoryItem);

                                reader.Close();
                            }
                        }
                    }
                }                     
            }
            catch (Exception)
            {
                // do some logging here
            }

            return inventoryInfo;
        }

        static async Task<List<ItemInventoryType>> Decrement(string sku)
        {
            List<ItemInventoryType> inventoryInfo = new List<ItemInventoryType>();

            try
            {                
                string updateSql = $"UPDATE Inventory SET Quantity = i.Quantity - 1 FROM Inventory as i where i.sku = '{sku}'";
                var selectSql = $"SELECT Sku, Quantity, Modified FROM Inventory WHERE Sku = '{sku}'";

                string sqlConnectionString = Environment.GetEnvironmentVariable("sqlConnectionString");

                using (SqlConnection conn = new SqlConnection(sqlConnectionString))
                {
                    await conn.OpenAsync();
                    
                    using (SqlCommand cmd = new SqlCommand(updateSql, conn))
                    {
                        await cmd.ExecuteNonQueryAsync();    

                        cmd.CommandText = selectSql;
                        var reader = await cmd.ExecuteReaderAsync();
                        
                        if (reader.HasRows)
                        {
                            if (await reader.ReadAsync())
                            {
                                var inventoryItem = new ItemInventoryType
                                {
                                    Sku = reader.GetString(0),
                                    Quantity = reader.GetInt32(1),
                                    Modified = reader.GetDateTime(2)
                                };

                                inventoryInfo.Add(inventoryItem);

                                reader.Close();
                            }
                        }
                    }
                }                                
            }             
            catch (Exception)
            {                
                // Do some logging here
            }

            return inventoryInfo;
        }
    }
}
