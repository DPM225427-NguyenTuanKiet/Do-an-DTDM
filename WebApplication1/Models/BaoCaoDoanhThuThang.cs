using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CarShop.Models
{
    public class BaoCaoDoanhThuThang
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("IDBCT")]
        public int IDBCT { get; set; }

        [BsonElement("THANG")]
        public int THANG { get; set; }

        [BsonElement("NAM")]
        public int NAM { get; set; }

        [BsonElement("TONG_DOANHTHU")]
        public decimal? TONG_DOANHTHU { get; set; }

        [BsonElement("TONG_DONHANG")]
        public int? TONG_DONHANG { get; set; }

        [BsonElement("TONG_LOINHUAN")]
        public decimal? TONG_LOINHUAN { get; set; }
    }
}