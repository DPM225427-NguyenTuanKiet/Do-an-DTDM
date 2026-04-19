using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CarShop.Models
{
    public class HoaDon
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("IDHD")]
        public int IDHD { get; set; }

        [BsonElement("IDDH")]
        public int? IDDH { get; set; }

        [BsonElement("IDNV")]
        public int? IDNV { get; set; }

        [BsonElement("NGAYLAP")]
        public DateTime? NGAYLAP { get; set; }

        [BsonElement("TONGTIEN")]
        public decimal? TONGTIEN { get; set; }

        [BsonElement("HINHTHUCTHANHTOAN")]
        public string? HINHTHUCTHANHTOAN { get; set; }

        [BsonElement("TRANGTHAI")]
        public string? TRANGTHAI { get; set; }

        [BsonElement("TRANGTHAITHANHTOAN")]
        public string? TRANGTHAITHANHTOAN { get; set; }
    }
}