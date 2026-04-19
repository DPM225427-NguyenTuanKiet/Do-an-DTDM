using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CarShop.Models
{
    public class NhanVien
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("IDNV")]
        public int IDNV { get; set; }

        [BsonElement("IDTK")]
        public int? IDTK { get; set; }

        [BsonElement("HOTEN")]
        public string? HOTEN { get; set; }

        [BsonElement("DIENTHOAI")]
        public string? DIENTHOAI { get; set; }

        [BsonElement("DIACHI")]
        public string? DIACHI { get; set; }

        [BsonElement("CHUCVU")]
        public string? CHUCVU { get; set; }

        [BsonElement("LUONGCB")]
        public decimal? LUONGCB { get; set; }

        [BsonElement("GIOLAM")]
        public int? GIOLAM { get; set; }

        [BsonElement("GIONGHI")]
        public int? GIONGHI { get; set; }

        [BsonElement("TONGLUONG")]
        public decimal? TONGLUONG { get; set; }

        [BsonElement("NGAYVAOLAM")]
        public DateTime? NGAYVAOLAM { get; set; }

        [BsonElement("IDCV")]
        public int? IDCV { get; set; }
    }
}