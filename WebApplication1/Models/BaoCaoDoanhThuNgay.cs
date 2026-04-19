using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CarShop.Models
{
    public class BaoCaoDoanhThuNgay
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("IDBC")]
        public int IDBC { get; set; }

        [BsonElement("NGAY")]
        public DateTime NGAY { get; set; }

        [BsonElement("TONG_DOANHTHU")]
        public decimal? TONG_DOANHTHU { get; set; }

        [BsonElement("TONG_DONHANG")]
        public int? TONG_DONHANG { get; set; }

        [BsonElement("TONG_LOINHUAN")]
        public decimal? TONG_LOINHUAN { get; set; }
    }
}