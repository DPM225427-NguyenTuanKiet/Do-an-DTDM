using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CarShop.Models
{
    public class DiaChiGiao
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("IDDC")]
        public int IDDC { get; set; }

        [BsonElement("IDKH")]
        public int IDKH { get; set; }

        [BsonElement("DIACHI")]
        public string DIACHI { get; set; } = null!;

        [BsonElement("MACDINH")]
        public bool? MACDINH { get; set; }
    }
}