using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CarShop.Models
{
    public class KhachHang
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("IDKH")]
        public int IDKH { get; set; }

        [BsonElement("IDTK")]
        public int? IDTK { get; set; }

        [BsonElement("HOTEN")]
        public string HOTEN { get; set; } = null!;

        [BsonElement("DIENTHOAI")]
        public string DIENTHOAI { get; set; } = null!;

        [BsonElement("DIACHI")]
        public string DIACHI { get; set; } = null!;

        [BsonElement("EMAIL")]
        public string EMAIL { get; set; } = null!;

        [BsonElement("IDLG")]
        public int IDLG { get; set; }

        [BsonElement("AVATAR")]
        public string? AVATAR { get; set; }
    }
}