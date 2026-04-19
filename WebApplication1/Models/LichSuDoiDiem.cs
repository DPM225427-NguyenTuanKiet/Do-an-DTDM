using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CarShop.Models
{
    public class LichSuDoiDiem
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("ID")]
        public int ID { get; set; }

        [BsonElement("IDKH")]
        public int IDKH { get; set; }

        [BsonElement("IDDONHANG")]
        public int? IDDONHANG { get; set; }

        [BsonElement("IDV")]
        public int? IDV { get; set; }

        [BsonElement("IDQT")]
        public int? IDQT { get; set; }

        [BsonElement("SOTIENGIAM")]
        public decimal? SOTIENGIAM { get; set; }

        [BsonElement("SODIEM_DOI")]
        public int SODIEM_DOI { get; set; }

        [BsonElement("NGAYDOI")]
        public DateTime NGAYDOI { get; set; }

        [BsonElement("TRANGTHAI")]
        public string TRANGTHAI { get; set; } = null!;
    }
}