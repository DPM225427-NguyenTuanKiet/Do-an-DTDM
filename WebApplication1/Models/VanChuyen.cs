using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CarShop.Models
{
    public class VanChuyen
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("IDVC")]
        public int IDVC { get; set; }

        [BsonElement("IDDH")]
        public int IDDH { get; set; }

        [BsonElement("DONVI")]
        public string? DONVI { get; set; }

        [BsonElement("MAVANDON")]
        public string? MAVANDON { get; set; }

        [BsonElement("NGAYGUI")]
        public DateTime? NGAYGUI { get; set; }

        [BsonElement("NGAYNHANDUKIEN")]
        public DateTime? NGAYNHANDUKIEN { get; set; }

        [BsonElement("PHIVANCHUYEN")]
        public decimal? PHIVANCHUYEN { get; set; }

        [BsonElement("TRANGTHAI")]
        public string? TRANGTHAI { get; set; }
    }
}