using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CarShop.Models
{
    public class NhaCungCap
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("IDNCC")]
        public int IDNCC { get; set; }

        [BsonElement("TENNCC")]
        public string? TENNCC { get; set; }

        [BsonElement("DIACHI")]
        public string? DIACHI { get; set; }

        [BsonElement("DIENTHOAI")]
        public string? DIENTHOAI { get; set; }

        [BsonElement("EMAIL")]
        public string? EMAIL { get; set; }
    }
}