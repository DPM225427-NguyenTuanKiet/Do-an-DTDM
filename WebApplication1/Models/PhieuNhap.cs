using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CarShop.Models
{
    public class PhieuNhap
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("IDPN")]
        public int IDPN { get; set; }

        [BsonElement("NGAYNHAP")]
        public DateTime NGAYNHAP { get; set; }

        [BsonElement("IDNCC")]
        public int IDNCC { get; set; }

        [BsonElement("IDNV")]
        public int IDNV { get; set; }

        [BsonElement("IDKHO")]
        public int IDKHO { get; set; }

        [BsonElement("CHITIET")]
        public List<ChiTietPhieuNhap> CHITIET { get; set; } = new();
    }

    public class ChiTietPhieuNhap
    {
        [BsonElement("IDSP")]
        public int IDSP { get; set; }

        [BsonElement("SOLUONG")]
        public int SOLUONG { get; set; }

        [BsonElement("DONGIA")]
        public decimal DONGIA { get; set; }
    }
}