using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CarShop.Models
{
    public class GioHang
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("IDGH")]
        public int IDGH { get; set; }

        [BsonElement("IDKH")]
        public int IDKH { get; set; }

        [BsonElement("NGAYTAO")]
        public DateTime NGAYTAO { get; set; }

        [BsonElement("SANPHAM")]
        public List<GioHangItem> SANPHAM { get; set; } = new();
    }

    public class GioHangItem
    {
        [BsonElement("IDSP")]
        public int IDSP { get; set; }

        [BsonElement("SOLUONG")]
        public int SOLUONG { get; set; }
    }
}