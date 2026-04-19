using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CarShop.Models
{
    public class TaiKhoan
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("IDTK")]
        public int IDTK { get; set; }

        [BsonElement("TENDANGNHAP")]
        public string TENDANGNHAP { get; set; } = null!;

        [BsonElement("MATKHAU")]
        public string MATKHAU { get; set; } = null!;

        [BsonElement("VAITRO")]
        public string VAITRO { get; set; } = null!;

        [BsonElement("EMAIL")]
        public string EMAIL { get; set; } = null!;

        [BsonElement("TRANGTHAI")]
        public bool TRANGTHAI { get; set; }

        [BsonElement("NGAYTAO")]
        public DateTime NGAYTAO { get; set; }

        // Thêm hai trường mới cho chức năng quên mật khẩu
        [BsonElement("ResetToken")]
        public string? ResetToken { get; set; }

        [BsonElement("ResetTokenExpiry")]
        public DateTime? ResetTokenExpiry { get; set; }
    }
}