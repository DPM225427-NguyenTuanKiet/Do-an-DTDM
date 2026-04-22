using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CarShop.Models
{
    public class DanhMuc
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("IDDM")]
        public int IDDM { get; set; }

        [BsonElement("TENDANHMUC")]
        public string TENDANHMUC { get; set; } = null!;
        [BsonElement("HINHANH")]
        public string? HINHANH { get; set; }
    }
}
