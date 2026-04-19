using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CarShop.Models
{
    public class ThanhToan
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("IDTT")]
        public int IDTT { get; set; }

        [BsonElement("IDDH")]
        public int IDDH { get; set; }

        [BsonElement("SOTIEN")]
        public decimal SOTIEN { get; set; }

        [BsonElement("HINHTHUC")]
        public string? HINHTHUC { get; set; }

        [BsonElement("MAGIAODICH")]
        public string? MAGIAODICH { get; set; }

        [BsonElement("NGAYTHANHTOAN")]
        public DateTime? NGAYTHANHTOAN { get; set; }

        [BsonElement("TRANGTHAI")]
        public string? TRANGTHAI { get; set; }
    }
}