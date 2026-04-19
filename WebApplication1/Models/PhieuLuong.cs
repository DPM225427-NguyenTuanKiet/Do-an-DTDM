using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CarShop.Models
{
    public class PhieuLuong
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("IDPL")]
        public int IDPL { get; set; }

        [BsonElement("IDNV")]
        public int? IDNV { get; set; }

        [BsonElement("THANG")]
        public int? THANG { get; set; }

        [BsonElement("NAM")]
        public int? NAM { get; set; }

        [BsonElement("LUONG_CO_BAN")]
        public decimal? LUONG_CO_BAN { get; set; }

        [BsonElement("HOA_HONG")]
        public decimal? HOA_HONG { get; set; }

        [BsonElement("TIEN_THUONG_DIEM")]
        public decimal? TIEN_THUONG_DIEM { get; set; }

        [BsonElement("SO_GIO_LAM")]
        public int? SO_GIO_LAM { get; set; }

        [BsonElement("TONG_LINH")]
        public decimal? TONG_LINH { get; set; }

        [BsonElement("NGAY_THANH_TOAN")]
        public DateTime? NGAY_THANH_TOAN { get; set; }

        [BsonElement("TRANG_THAI")]
        public string? TRANG_THAI { get; set; }
    }
}