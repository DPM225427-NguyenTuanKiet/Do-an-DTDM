using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CarShop.Models
{
    public class SanPhamKhuyenMai
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("IDSP")]
        public int IDSP { get; set; }

        [BsonElement("IDKM")]
        public int IDKM { get; set; }
    }
}