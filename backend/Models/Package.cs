using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace WebApplication1.Models
{
    public class Package
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; } // Auto-generated

        [BsonElement("Lecturer_Email")]
        public string LecturerEmail { get; set; } = string.Empty;

        [BsonElement("ItemCount")]
        public int ItemCount { get; set; }

        [BsonElement("ShippingProvider")]
        public string? ShippingProvider { get; set; } = string.Empty;

        [BsonElement("AdditionalInfo")]
        public string? AdditionalInfo { get; set; } = string.Empty;

        [BsonElement("CollectionDate")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)] // Ensure UTC format
        public DateTime? CollectionDate { get; set; }

        [BsonElement("Status")]
        public string? Status { get; set; }
    }
}