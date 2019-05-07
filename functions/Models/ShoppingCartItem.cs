using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Attributes;

using System;

namespace TailwindTraders.Functions
{
    public class ShoppingCartItem
    {        
        [BsonId(IdGenerator = typeof(ObjectIdGenerator))]
        public MongoDB.Bson.ObjectId Id { get; set; }

        [BsonElement("id")]
        public string ProductId { get; set; }

        [BsonElement("sku")]
        public string Sku { get; set; }

        [BsonElement("userId")]
        public string UserId { get; set; }

        [BsonElement("quantity")]
        public int Quantity { get; set; }

        [BsonElement("dateModified")]
        public DateTime DateModified { get; set; }
    }
}
