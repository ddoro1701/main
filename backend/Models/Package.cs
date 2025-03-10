using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace WebApplication1.Models
{
    public class Package
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty; // Initialisieren

        [BsonElement("RecipientName")]
        public string RecipientName { get; set; } = string.Empty; // Initialisieren

        [BsonElement("TrackingNumber")]
        public string TrackingNumber { get; set; } = string.Empty; // Initialisieren

        [BsonElement("ArrivalDate")]
        public DateTime ArrivalDate { get; set; }

        [BsonElement("LecturerEmail")]
        public string LecturerEmail { get; set; } = string.Empty; // Initialisieren

        [BsonElement("Status")]
        public string Status { get; set; } = "Pending"; // Standardstatus: "Pending"
    }
}