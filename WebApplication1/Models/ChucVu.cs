using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CarShop.Models
{
    public class ChucVu
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("IDCV")]
        public int IDCV { get; set; }

        [BsonElement("TENCV")]
        public string TENCV { get; set; } = null!;

        [BsonElement("MOTA")]
        public string? MOTA { get; set; }
    }
}