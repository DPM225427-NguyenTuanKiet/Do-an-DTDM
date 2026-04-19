using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CarShop.Models
{
    public class LoaiGia
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("IDLG")]
        public int IDLG { get; set; }

        [BsonElement("TENLOAI")]
        public string TENLOAI { get; set; } = null!;

        [BsonElement("DIEMTHUONG")]
        public int DIEMTHUONG { get; set; }

        [BsonElement("GIAMGIA")]
        public decimal GIAMGIA { get; set; }

        [BsonElement("DIEM_TOITHIEU")]
        public int DIEM_TOITHIEU { get; set; }
    }
}