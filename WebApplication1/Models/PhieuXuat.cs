using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CarShop.Models
{
    public class PhieuXuat
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("IDPX")]
        public int IDPX { get; set; }

        [BsonElement("NGAYXUAT")]
        public DateTime NGAYXUAT { get; set; }

        [BsonElement("IDNV")]
        public int IDNV { get; set; }

        [BsonElement("IDKHO")]
        public int IDKHO { get; set; }

        [BsonElement("IDDH")]
        public int? IDDH { get; set; }

        [BsonElement("LOAIPHIEU")]
        public string LOAIPHIEU { get; set; } = null!;

        [BsonElement("CHITIET")]
        public List<ChiTietPhieuXuat> CHITIET { get; set; } = new();
    }

    public class ChiTietPhieuXuat
    {
        [BsonElement("IDSP")]
        public int IDSP { get; set; }

        [BsonElement("SOLUONG")]
        public int SOLUONG { get; set; }
    }
}