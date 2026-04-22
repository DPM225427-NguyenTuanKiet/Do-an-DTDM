using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace CarShop.Models
{
    public class BaoHanh
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("IDBH")]
        public int IDBH { get; set; }

        [BsonElement("IDSP")]
        public int IDSP { get; set; }

        [BsonElement("IDKH")]
        public int IDKH { get; set; }

        [BsonElement("IDDH")]
        public int IDDH { get; set; }

        [BsonElement("NGAYBATDAU")]
        public DateTime? NGAYBATDAU { get; set; }

        [BsonElement("NGAYKETTHUC")]
        public DateTime? NGAYKETTHUC { get; set; }

        [BsonElement("TRANGTHAI")]
        public string? TRANGTHAI { get; set; }

        // BỔ SUNG TRƯỜNG NÀY ĐỂ FIX LỖI
        [BsonElement("MOTA")]
        public string? MOTA { get; set; }

        [BsonElement("LICHSU")]
        public List<LichSuBaoHanh> LICHSU { get; set; } = new();
    }

    public class LichSuBaoHanh
    {
        [BsonElement("IDLS")]
        public int IDLS { get; set; }

        [BsonElement("NGAYTIEPNHAN")]
        public DateTime? NGAYTIEPNHAN { get; set; }

        [BsonElement("NGAYTRA")]
        public DateTime? NGAYTRA { get; set; }

        [BsonElement("NOIDUNG")]
        public string? NOIDUNG { get; set; }

        [BsonElement("TINHTRANG")]
        public string? TINHTRANG { get; set; }

        [BsonElement("CHIPHI")]
        public decimal? CHIPHI { get; set; }
    }
}