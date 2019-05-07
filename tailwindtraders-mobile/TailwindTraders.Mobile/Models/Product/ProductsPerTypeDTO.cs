using System;
using System.Collections.Generic;

using System.Globalization;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;


namespace TailwindTraders.Mobile.Features.Product
{
    public partial class ProductsPerTypeDTO
    {
        [JsonProperty("items")]
        public List<ProductDTO> Products { get; set; }

        [JsonProperty("size")]
        public long Size { get; set; }
    }

    public partial class ProductDTO
    {
        [JsonProperty("_id")]
        public string Id { get; set; }

        [JsonProperty("id")]
        public long ItemId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("price")]
        public float Price { get; set; }

        [JsonProperty("productType")]
        public ProductType ProductType { get; set; }

        [JsonProperty("supplierName")]
        public SupplierName SupplierName { get; set; }

        [JsonProperty("sku")]
        public string Sku { get; set; }

        [JsonProperty("shortDescription")]
        public string ShortDescription { get; set; }

        [JsonProperty("longDescription")]
        public string LongDescription { get; set; }

        [JsonProperty("digital")]
        public string Digital { get; set; }

        [JsonProperty("unitDescription")]
        public string UnitDescription { get; set; }

        [JsonProperty("dimensions")]
        public string Dimensions { get; set; }

        [JsonProperty("weightInPounds")]
        public string WeightInPounds { get; set; }

        [JsonProperty("reorder_amount")]
        public string ReorderAmount { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("location")]
        public string Location { get; set; }

        [JsonProperty("images")]
        public List<Image> Images { get; set; }

        [JsonIgnore]
        public string ImageUrl => Images?.FirstOrDefault()?.Url?.ToString() ?? string.Empty;
    }

    public partial class Image
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("caption")]
        public string Caption { get; set; }

        [JsonProperty("url")]
        public Uri Url { get; set; }
    }

    public enum ProductType { Electrical, Hardware, Hinges, Tiles };

    public enum SupplierName { Northwind, TailwindWholesale };

    public partial class ProductsPerTypeDTO
    {
        public static ProductsPerTypeDTO FromJson(string json) => JsonConvert.DeserializeObject<ProductsPerTypeDTO>(json, TailwindTraders.Mobile.Features.Product.ProductPerDTOJsonConverter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this ProductsPerTypeDTO self) => JsonConvert.SerializeObject(self, TailwindTraders.Mobile.Features.Product.ProductPerDTOJsonConverter.Settings);
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
