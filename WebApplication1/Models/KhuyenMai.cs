using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CarShop.Models
{
    public class KhuyenMai
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("IDKM")]
        public int IDKM { get; set; }

        [BsonElement("TENKM")]
        public string? TENKM { get; set; }

        [BsonElement("PHANTRAMGIAM")]
        public int? PHANTRAMGIAM { get; set; }

        [BsonElement("NGAYBATDAU")]
        public DateTime? NGAYBATDAU { get; set; }

        [BsonElement("NGAYKETTHUC")]
        public DateTime? NGAYKETTHUC { get; set; }

        [BsonElement("TRANGTHAI")]
        public bool? TRANGTHAI { get; set; }
    }
}