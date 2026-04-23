using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CarShop.Models
{
    public class TonKho
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("IDKHO")]
        public int IDKHO { get; set; }

        [BsonElement("IDSP")]
        public int IDSP { get; set; }

        [BsonElement("SOLUONGTON")]
        public int SOLUONGTON { get; set; }
    }
}