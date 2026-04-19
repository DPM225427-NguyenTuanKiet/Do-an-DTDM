using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CarShop.Models
{
    public class YeuThich
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("IDYeuThich")]
        public int IDYeuThich { get; set; }

        [BsonElement("IDKH")]
        public int IDKH { get; set; }

        [BsonElement("IDSP")]
        public int IDSP { get; set; }

        [BsonElement("NGAYTAO")]
        public DateTime? NGAYTAO { get; set; }
    }
}