using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CarShop.Models
{
    public class LichSuDiem
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("ID")]
        public int ID { get; set; }

        [BsonElement("IDKH")]
        public int IDKH { get; set; }

        [BsonElement("IDDH")]
        public int? IDDH { get; set; }

        [BsonElement("SODIEM")]
        public int SODIEM { get; set; }

        [BsonElement("LOAI")]
        public string LOAI { get; set; } = null!;

        [BsonElement("NGAY")]
        public DateTime? NGAY { get; set; }

        [BsonElement("GHICHU")]
        public string? GHICHU { get; set; }
    }
}