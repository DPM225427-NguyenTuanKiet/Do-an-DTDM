using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CarShop.Models
{
    public class Token
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("IDTOKEN")]
        public int IDTOKEN { get; set; }

        [BsonElement("IDTK")]
        public int IDTK { get; set; }

        [BsonElement("TOKEN")]
        public string TOKEN { get; set; } = null!;

        [BsonElement("NGAYHETHAN")]
        public DateTime NGAYHETHAN { get; set; }

        [BsonElement("NGAYTAO")]
        public DateTime? NGAYTAO { get; set; }
    }
}