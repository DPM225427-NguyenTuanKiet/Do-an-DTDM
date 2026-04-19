using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CarShop.Models
{
    public class LichSuDonHang
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("IDLS")]
        public int IDLS { get; set; }

        [BsonElement("IDDH")]
        public int IDDH { get; set; }

        [BsonElement("TRANGTHAICU")]
        public string? TRANGTHAICU { get; set; }

        [BsonElement("TRANGTHAIMOI")]
        public string TRANGTHAIMOI { get; set; } = null!;

        [BsonElement("NGAYTHAYDOI")]
        public DateTime? NGAYTHAYDOI { get; set; }

        [BsonElement("NGUOITHAYDOI")]
        public int? NGUOITHAYDOI { get; set; }
    }
}