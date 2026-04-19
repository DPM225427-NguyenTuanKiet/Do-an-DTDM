using MongoDB.Driver;
using CarShop.Models;

namespace CarShop.Services
{
    public class GioHangService
    {
        private readonly IMongoCollection<GioHang> _collection;

        public GioHangService(MongoDbContext dbContext)
        {
            _collection = dbContext.GioHang;
        }

        public async Task<GioHang?> GetByKhachHangIdAsync(int khachHangId) =>
            await _collection.Find(x => x.IDKH == khachHangId).FirstOrDefaultAsync();

        public async Task UpdateAsync(GioHang gioHang) =>
            await _collection.ReplaceOneAsync(x => x.IDKH == gioHang.IDKH, gioHang, new ReplaceOptions { IsUpsert = true });
    }
}