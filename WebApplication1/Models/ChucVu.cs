using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CarShop.Models
{
    [BsonIgnoreExtraElements]
    public class ChucVu
    {
        // 1. Thêm trường này để chứa _id mặc định của MongoDB
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        // 2. Bỏ thẻ [BsonId] ở IDCV đi, để nó làm 1 cột bình thường
        public int IDCV { get; set; }

        public string TENCV { get; set; } = null!;
        public string? MOTA { get; set; }
    }
}