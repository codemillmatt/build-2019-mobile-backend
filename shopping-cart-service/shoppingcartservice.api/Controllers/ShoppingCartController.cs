using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using ShoppingCartService.api.DataService;
using ShoppingCartService.api.Models;

namespace ShoppingCartService.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShoppingCartController
    {
        IMongoService mongo;

        public ShoppingCartController(IMongoService mongoService)
        {
            mongo = mongoService;
        }
        
        // GET api/values/username
        [HttpGet("{userId}")]
        public async Task<ActionResult<IEnumerable<ShoppingCartItem>>> Get(string userId)
        {
            var m = await mongo.GetCartItemsForUser(userId);

            return new OkObjectResult(m);
        }

        // PUT api/values/5
        [HttpPost("{userId}")]
        public async Task<ActionResult> Put(string userId, [FromBody] BareProductInfo value)
        {
            try
            {
                var cartItem = new ShoppingCartItem { 
                    DateModified = DateTime.UtcNow,
                    UserId = userId,
                    ProductId = value.ProductId,
                    Sku = value.Sku,
                    Quantity = 1
                };

                await mongo.AddCartItem(cartItem);

                return new OkResult();
            } 
            catch (Exception)
            {
                return new StatusCodeResult(500);
            }
        }
    
        [HttpDelete("{userId}")]
        public async Task<ActionResult> Delete(string userId, [FromBody]BareProductInfo value)
        {
            try
            {
                long.TryParse(value.ProductId, out long productId);

                await mongo.RemoveCartItem(productId, userId);

                return new OkResult();
            }
            catch (Exception)
            {
                return new StatusCodeResult(500);
            }
        }

        [HttpDelete("removeall/{userId}")]
        public async Task<ActionResult> DeleteAllItemsForUser(string userId)
        {
            try 
            {
                await mongo.RemoveAllCartItemsForUser(userId);

                return new OkResult();
            }
            catch (Exception)
            {
                return new StatusCodeResult (500);
            }

        }

        [HttpPost("{userId}/purchase")]
        public async Task<ActionResult> PurchaseCartItems(string userId, 
            [FromBody]List<BareProductInfo> prodsToPurchase)
        {
            var shoppingCartProducts = await mongo.GetCartItemsForUser(userId);

            var notInProdsToPurchase = shoppingCartProducts.Count(sci => !prodsToPurchase.Any(ptp => ptp.ProductId == sci.ProductId));
            var notInCart = prodsToPurchase.Count(ptp => !shoppingCartProducts.Any(sci => sci.ProductId == ptp.ProductId));

            var returnInfo = ValueTuple.Create(shoppingCartProducts, prodsToPurchase);

            if (notInProdsToPurchase > 0 || notInCart > 0)
                return new ConflictObjectResult(returnInfo);

            await mongo.RemoveAllCartItemsForUser(userId);

            return new OkObjectResult(returnInfo);
        }    
    }
}