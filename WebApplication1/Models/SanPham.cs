using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CarShop.Models
{
    public class SanPham
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("IDSP")]
        public int IDSP { get; set; }

        [BsonElement("TENSP")]
        public string TENSP { get; set; } = null!;

        [BsonElement("GIA")]
        public decimal GIA { get; set; }

        [BsonElement("SOLUONG")]
        public int SOLUONG { get; set; }

        [BsonElement("MOTA")]
        public string? MOTA { get; set; }

        [BsonElement("IDDM")]
        public int IDDM { get; set; }

        [BsonElement("IDTH")]
        public int IDTH { get; set; }

        [BsonElement("NGAYTAO")]
        public DateTime NGAYTAO { get; set; }

        [BsonElement("TRANGTHAI")]
        public bool TRANGTHAI { get; set; }

        [BsonElement("TONKHO_TOITHIEU")]
        public int TONKHO_TOITHIEU { get; set; }

        [BsonIgnore]
        public DanhMuc? DanhMuc { get; set; }
        [BsonIgnore]
        public ThuongHieu? ThuongHieu { get; set; }
        [BsonIgnore]
        public List<HinhAnh> HinhAnh { get; set; } = new();
    }
}