using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CarShop.Models
{
    public class ThuongHieu
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("IDTH")]
        public int IDTH { get; set; }

        [BsonElement("TENTHUONGHIEU")]
        public string TENTHUONGHIEU { get; set; } = null!;
        [BsonElement("HINHANH")]
        public string? HINHANH { get; set; }

        [BsonElement("QUOCGIA")]
        public string? QUOCGIA { get; set; }
    }
}