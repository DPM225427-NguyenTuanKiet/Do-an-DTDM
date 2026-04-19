using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CarShop.Models
{
    public class GiaKhuyenMai
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("IDGKM")]
        public int IDGKM { get; set; }

        [BsonElement("IDSP")]
        public int IDSP { get; set; }

        [BsonElement("GIAKHUYENMAI")]
        public decimal GIAKHUYENMAI { get; set; }

        [BsonElement("NGAYBATDAU")]
        public DateTime NGAYBATDAU { get; set; }

        [BsonElement("NGAYKETTHUC")]
        public DateTime NGAYKETTHUC { get; set; }
    }
}