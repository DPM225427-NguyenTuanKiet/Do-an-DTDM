using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CarShop.Models
{
    public class HinhAnh
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("IDHINHANH")]
        public int IDHINHANH { get; set; }

        [BsonElement("IDSP")]
        public int? IDSP { get; set; }

        [BsonElement("DUONGDAN")]
        public string? DUONGDAN { get; set; }
    }
}
