using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CarShop.Models
{
    public class QuaTang
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("IDQT")]
        public int IDQT { get; set; }

        [BsonElement("TENQUATANG")]
        public string TENQUATANG { get; set; } = null!;

        [BsonElement("DIEM_DOI")]
        public int DIEM_DOI { get; set; }

        [BsonElement("SOLUONG")]
        public int SOLUONG { get; set; }

        [BsonElement("MOTA")]
        public string? MOTA { get; set; }

        [BsonElement("HINHANH")]
        public string? HINHANH { get; set; }

        [BsonElement("TRANGTHAI")]
        public bool TRANGTHAI { get; set; }
    }
}