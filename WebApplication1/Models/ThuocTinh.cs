using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CarShop.Models
{
    public class ThuocTinh
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("IDTT")]
        public int IDTT { get; set; }

        [BsonElement("TENTHUOCTINH")]
        public string TENTHUOCTINH { get; set; } = null!;
    }
}
