using MongoDB.Driver;
using CarShop.Models;

namespace CarShop.Services
{
    public class PhieuLuongService
    {
        private readonly IMongoCollection<PhieuLuong> _collection;
        public PhieuLuongService(MongoDbContext dbContext) => _collection = dbContext.PhieuLuong;

        public async Task<List<PhieuLuong>> GetAllAsync() => await _collection.Find(_ => true).ToListAsync();
        public async Task<PhieuLuong?> GetByIdAsync(int id) => await _collection.Find(x => x.IDPL == id).FirstOrDefaultAsync();
        public async Task<List<PhieuLuong>> GetByNhanVienIdAsync(int idNV) => await _collection.Find(x => x.IDNV == idNV).ToListAsync();
        public async Task CreateAsync(PhieuLuong entity) => await _collection.InsertOneAsync(entity);
        public async Task UpdateAsync(int id, PhieuLuong entity) => await _collection.ReplaceOneAsync(x => x.IDPL == id, entity);
        public async Task DeleteAsync(int id) => await _collection.DeleteOneAsync(x => x.IDPL == id);
    }
}