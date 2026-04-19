using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CarShop.Models
{
    public class YeuCauTraHang
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("IDYCTH")]
        public int IDYCTH { get; set; }

        [BsonElement("IDDH")]
        public int IDDH { get; set; }

        [BsonElement("IDKH")]
        public int IDKH { get; set; }

        [BsonElement("NGAYYEUCAU")]
        public DateTime? NGAYYEUCAU { get; set; }

        [BsonElement("LYDO")]
        public string? LYDO { get; set; }

        [BsonElement("TRANGTHAI")]
        public string? TRANGTHAI { get; set; }

        [BsonElement("CHITIET")]
        public List<ChiTietTraHang> CHITIET { get; set; } = new();
    }

    public class ChiTietTraHang
    {
        [BsonElement("IDSP")]
        public int IDSP { get; set; }

        [BsonElement("SOLUONG")]
        public int SOLUONG { get; set; }

        [BsonElement("DONGIA")]
        public decimal? DONGIA { get; set; }

        [BsonElement("TIENHOAN")]
        public decimal? TIENHOAN { get; set; }
    }
}