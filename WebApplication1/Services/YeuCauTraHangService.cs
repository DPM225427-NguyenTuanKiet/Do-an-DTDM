using MongoDB.Driver;
using CarShop.Models;

namespace CarShop.Services
{
    public class YeuCauTraHangService
    {
        private readonly IMongoCollection<YeuCauTraHang> _collection;
        public YeuCauTraHangService(MongoDbContext dbContext) => _collection = dbContext.YeuCauTraHang;
        public async Task<List<YeuCauTraHang>> GetAllAsync() => await _collection.Find(_ => true).ToListAsync();
        public async Task<YeuCauTraHang?> GetByIdAsync(int id) => await _collection.Find(x => x.IDYCTH == id).FirstOrDefaultAsync();
        public async Task<List<YeuCauTraHang>> GetByKhachHangIdAsync(int idKH) => await _collection.Find(x => x.IDKH == idKH).ToListAsync();
        public async Task CreateAsync(YeuCauTraHang entity) => await _collection.InsertOneAsync(entity);
        public async Task UpdateAsync(int id, YeuCauTraHang entity) => await _collection.ReplaceOneAsync(x => x.IDYCTH == id, entity);
        public async Task DeleteAsync(int id) => await _collection.DeleteOneAsync(x => x.IDYCTH == id);
    }
}