using System;
using System.Collections.Generic;

using System.Globalization;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Attributes;


namespace TailwindTraders.Functions
{
    public partial class ProductsPerTypeDTO
    {
        [JsonProperty("items")]
        public List<ProductDTO> Products { get; set; }

        [JsonProperty("size")]
        public long Size { get; set; }
    }

    [BsonIgnoreExtraElements]
    public partial class ProductDTO
    {
        [BsonIgnore]
        public string _id { get; set; }


        [JsonProperty("id")]
        [BsonElement("id")]
        public long ItemId { get; set; }

        [JsonProperty("name")]
        [BsonElement("name")]
        public string Name { get; set; }

        [JsonProperty("price")]
        [BsonElement("price")]
        public float Price { get; set; }

        [JsonProperty("productType")]
        [BsonElement("productType")]
        public string ProductType { get; set; }

        [JsonProperty("supplierName")]
        [BsonElement("supplierName")]
        public string SupplierName { get; set; }

        [JsonProperty("sku")]
        [BsonElement("sku")]
        public string Sku { get; set; }

        [JsonProperty("shortDescription")]
        [BsonElement("shortDescription")]
        public string ShortDescription { get; set; }

        [JsonProperty("longDescription")]
        [BsonElement("longDescription")]
        public string LongDescription { get; set; }

        [JsonProperty("digital")]
        [BsonElement("digital")]
        public string Digital { get; set; }

        [JsonProperty("unitDescription")]
        [BsonElement("unitDescription")]
        public string UnitDescription { get; set; }

        [JsonProperty("dimensions")]
        [BsonElement("dimensions")]
        public string Dimensions { get; set; }

        [JsonProperty("weightInPounds")]
        [BsonElement("weightInPounds")]
        public string WeightInPounds { get; set; }

        [JsonProperty("reorder_amount")]
        [BsonElement("reorder_amount")]
        public string ReorderAmount { get; set; }

        [JsonProperty("status")]
        [BsonElement("status")]
        public string Status { get; set; }

        [JsonProperty("location")]
        [BsonElement("location")]
        public string Location { get; set; }

        [JsonProperty("images")]
        [BsonElement("images")]
        public List<Image> Images { get; set; }

        [JsonIgnore]
        public string ImageUrl => Images?.FirstOrDefault()?.Url?.ToString() ?? string.Empty;
    }

    [BsonIgnoreExtraElements]
    public partial class Image
    {
        [JsonProperty("id")]
        [BsonElement("id")]
        public long Id { get; set; }

        [JsonProperty("caption")]
        [BsonElement("caption")]
        public string Caption { get; set; }

        [JsonProperty("url")]
        [BsonElement("url")]
        public Uri Url { get; set; }
    }

    public enum ProductType { Electrical, Hardware, Hinges, Tiles };

    public enum SupplierName { Northwind, TailwindWholesale };

    public partial class ProductsPerTypeDTO
    {
        public static ProductsPerTypeDTO FromJson(string json) => JsonConvert.DeserializeObject<ProductsPerTypeDTO>(json, TailwindTraders.Functions.ProductPerDTOJsonConverter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this ProductsPerTypeDTO self) => JsonConvert.SerializeObject(self, TailwindTraders.Functions.ProductPerDTOJsonConverter.Settings);
    }

    internal static class ProductPerDTOJsonConverter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                ProductTypeConverter.Singleton,
                SupplierNameConverter.Singleton,
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class ProductTypeConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(ProductType) || t == typeof(ProductType?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "Electrical":
                    return ProductType.Electrical;
                case "Hardware":
                    return ProductType.Hardware;
                case "Hinges":
                    return ProductType.Hinges;
                case "Tiles":
                    return ProductType.Tiles;
            }
            throw new Exception("Cannot unmarshal type ProductType");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (ProductType)untypedValue;
            switch (value)
            {
                case ProductType.Electrical:
                    serializer.Serialize(writer, "Electrical");
                    return;
                case ProductType.Hardware:
                    serializer.Serialize(writer, "Hardware");
                    return;
                case ProductType.Hinges:
                    serializer.Serialize(writer, "Hinges");
                    return;
                case ProductType.Tiles:
                    serializer.Serialize(writer, "Tiles");
                    return;
            }
            throw new Exception("Cannot marshal type ProductType");
        }

        public static readonly ProductTypeConverter Singleton = new ProductTypeConverter();
    }

    internal class SupplierNameConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(SupplierName) || t == typeof(SupplierName?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "Northwind":
                    return SupplierName.Northwind;
                case "Tailwind Wholesale":
                    return SupplierName.TailwindWholesale;
            }
            throw new Exception("Cannot unmarshal type SupplierName");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (SupplierName)untypedValue;
            switch (value)
            {
                case SupplierName.Northwind:
                    serializer.Serialize(writer, "Northwind");
                    return;
                case SupplierName.TailwindWholesale:
                    serializer.Serialize(writer, "Tailwind Wholesale");
                    return;
            }
            throw new Exception("Cannot marshal type SupplierName");
        }

        public static readonly SupplierNameConverter Singleton = new SupplierNameConverter();
    }
}
