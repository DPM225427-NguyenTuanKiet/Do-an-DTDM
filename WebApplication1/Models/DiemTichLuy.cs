using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CarShop.Models
{
    public class DiemTichLuy
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("IDKH")]
        public int IDKH { get; set; }

        [BsonElement("DIEMHIENTAI")]
        public int? DIEMHIENTAI { get; set; }

        [BsonElement("TONGDIEMDADUNG")]
        public int? TONGDIEMDADUNG { get; set; }

        [BsonElement("NGAYCAPNHAT")]
        public DateTime? NGAYCAPNHAT { get; set; }
    }
}