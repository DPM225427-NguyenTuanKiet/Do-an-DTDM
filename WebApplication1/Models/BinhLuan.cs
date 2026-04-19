using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CarShop.Models
{
    public class BinhLuan
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("IDBL")]
        public int IDBL { get; set; }

        [BsonElement("IDSP")]
        public int IDSP { get; set; }

        [BsonElement("IDTK")]
        public int IDTK { get; set; }

        [BsonElement("NOIDUNG")]
        public string? NOIDUNG { get; set; }

        [BsonElement("NGAYTAO")]
        public DateTime? NGAYTAO { get; set; }

        [BsonElement("SOSAO")]
        public int? SOSAO { get; set; }
    }
}