using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CarShop.Models
{
    public class Kho
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("IDKHO")]
        public int IDKHO { get; set; }

        [BsonElement("TENKHO")]
        public string? TENKHO { get; set; }

        [BsonElement("DIACHI")]
        public string? DIACHI { get; set; }
    }
}