using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CarShop.Models
{
    public class DonHang
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("IDDH")]
        public int IDDH { get; set; }

        [BsonElement("IDKH")]
        public int IDKH { get; set; }

        [BsonElement("NGAYDAT")]
        public DateTime NGAYDAT { get; set; }

        [BsonElement("TONGTIEN")]
        public decimal TONGTIEN { get; set; }

        [BsonElement("TRANGTHAI")]
        public string TRANGTHAI { get; set; } = null!;

        [BsonElement("DA_DUYET")]
        public bool DA_DUYET { get; set; } = false;

        [BsonElement("DIACHIGIAO")]
        public string DIACHIGIAO { get; set; } = null!;

        [BsonElement("CHITIET")]
        public List<ChiTietDonHang> CHITIET { get; set; } = new();
    }

    public class ChiTietDonHang
    {
        [BsonElement("IDSP")]
        public int IDSP { get; set; }

        [BsonElement("SOLUONG")]
        public int SOLUONG { get; set; }

        [BsonElement("DONGIA")]
        public decimal DONGIA { get; set; }
    }
}