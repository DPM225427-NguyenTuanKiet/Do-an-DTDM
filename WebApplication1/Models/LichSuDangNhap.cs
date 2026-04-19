using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CarShop.Models
{
    public class LichSuDangNhap
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("IDLS")]
        public int IDLS { get; set; }

        [BsonElement("IDTK")]
        public int IDTK { get; set; }

        [BsonElement("THOIGIAN")]
        public DateTime? THOIGIAN { get; set; }

        [BsonElement("IP")]
        public string? IP { get; set; }

        [BsonElement("THIETBI")]
        public string? THIETBI { get; set; }

        [BsonElement("TRANGTHAI")]
        public bool? TRANGTHAI { get; set; }
    }
}