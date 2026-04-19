using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CarShop.Models
{
    public class GiaiThuocTinh
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("IDSP")]
        public int IDSP { get; set; }

        [BsonElement("IDTT")]
        public int IDTT { get; set; }

        [BsonElement("GIATRI")]
        public string GIATRI { get; set; } = null!;
    }
}