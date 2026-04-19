using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CarShop.Models
{
    public class ActivityLog
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("IDLOG")]
        public int IDLOG { get; set; }

        [BsonElement("IDTK")]
        public int? IDTK { get; set; }

        [BsonElement("THOIGIAN")]
        public DateTime? THOIGIAN { get; set; }

        [BsonElement("HANHDONG")]
        public string? HANHDONG { get; set; }

        [BsonElement("CHITIET")]
        public string? CHITIET { get; set; }

        [BsonElement("IP")]
        public string? IP { get; set; }
    }
}