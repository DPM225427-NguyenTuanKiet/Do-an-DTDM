using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CarShop.Models
{
    public class ThongSoKyThuat
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("IDTS")]
        public int IDTS { get; set; }

        [BsonElement("IDSP")]
        public int IDSP { get; set; }

        // Thông số động cơ
        [BsonElement("LOAI_DONG_CO")]
        public string? LOAI_DONG_CO { get; set; }

        [BsonElement("DUNG_TICH")]
        public string? DUNG_TICH { get; set; }

        [BsonElement("CONG_SUAT")]
        public string? CONG_SUAT { get; set; }

        [BsonElement("MO_MEN_XOAN")]
        public string? MO_MEN_XOAN { get; set; }

        [BsonElement("NHIEN_LIEU")]
        public string? NHIEN_LIEU { get; set; }

        [BsonElement("HOP_SO")]
        public string? HOP_SO { get; set; }

        [BsonElement("DAN_DONG")]
        public string? DAN_DONG { get; set; }

        // Tiêu thụ nhiên liệu
        [BsonElement("TIEU_THU_NHIEN_LIEU")]
        public string? TIEU_THU_NHIEN_LIEU { get; set; }

        // Kích thước & trọng lượng
        [BsonElement("KICH_THUOC")]
        public string? KICH_THUOC { get; set; }

        [BsonElement("KHOI_LUONG")]
        public string? KHOI_LUONG { get; set; }

        [BsonElement("SO_CHO")]
        public int? SO_CHO { get; set; }

        [BsonElement("KICH_THUOC_LOP")]
        public string? KICH_THUOC_LOP { get; set; }

        // Hệ thống treo & phanh
        [BsonElement("HE_THONG_TREO")]      // <-- THÊM TRƯỜNG NÀY
        public string? HE_THONG_TREO { get; set; }

        [BsonElement("HE_THONG_TREO_TRUOC")]
        public string? HE_THONG_TREO_TRUOC { get; set; }

        [BsonElement("HE_THONG_TREO_SAU")]
        public string? HE_THONG_TREO_SAU { get; set; }

        [BsonElement("PHANH")]              // <-- THÊM TRƯỜNG NÀY (nếu có)
        public string? PHANH { get; set; }

        [BsonElement("PHANH_TRUOC")]
        public string? PHANH_TRUOC { get; set; }

        [BsonElement("PHANH_SAU")]
        public string? PHANH_SAU { get; set; }

        [BsonElement("TRO_LUC_LAI")]
        public string? TRO_LUC_LAI { get; set; }

        // An toàn & tiện nghi
        [BsonElement("AN_TOAN")]
        public List<string>? AN_TOAN { get; set; }

        [BsonElement("GIAI_TRI")]
        public List<string>? GIAI_TRI { get; set; }

        [BsonElement("KET_NOI")]
        public string? KET_NOI { get; set; }

        // Dành cho xe điện
        [BsonElement("DUNG_TICH_PIN")]
        public string? DUNG_TICH_PIN { get; set; }

        [BsonElement("QUANG_DUONG_DIEN")]
        public string? QUANG_DUONG_DIEN { get; set; }

        // Các trường cũ (giữ lại để tương thích)
        [BsonElement("DO_PHAN_GIAI")]
        public string? DO_PHAN_GIAI { get; set; }

        [BsonElement("CAMBIEN")]
        public string? CAMBIEN { get; set; }

        [BsonElement("ONG_KINH")]
        public string? ONG_KINH { get; set; }

        [BsonElement("THE_NHO")]
        public string? THE_NHO { get; set; }

        [BsonElement("PIN")]
        public string? PIN { get; set; }
    }
}