using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CarShop.Models
{
    public class Voucher
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("IDV")]
        public int IDV { get; set; }

        [BsonElement("MAVOUCHER")]
        public string MAVOUCHER { get; set; } = null!;

        [BsonElement("TENVOUCHER")]
        public string TENVOUCHER { get; set; } = null!;

        [BsonElement("GIATRI")]
        public decimal GIATRI { get; set; }

        [BsonElement("LOAI")]
        public string LOAI { get; set; } = null!;

        [BsonElement("SOTIEN_TOITHIEU")]
        public decimal? SOTIEN_TOITHIEU { get; set; }

        [BsonElement("NGAYBATDAU")]
        public DateTime? NGAYBATDAU { get; set; }

        [BsonElement("NGAYKETTHUC")]
        public DateTime? NGAYKETTHUC { get; set; }

        [BsonElement("SOLUONG")]
        public int? SOLUONG { get; set; }

        [BsonElement("TRANGTHAI")]
        public bool TRANGTHAI { get; set; }
    }
}