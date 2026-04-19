using MongoDB.Driver;
using CarShop.Models;

namespace CarShop.Services
{
    public class KhuyenMaiService
    {
        private readonly IMongoCollection<KhuyenMai> _collection;
        public KhuyenMaiService(MongoDbContext dbContext) => _collection = dbContext.KhuyenMai;
        public async Task<List<KhuyenMai>> GetAllAsync() => await _collection.Find(_ => true).ToListAsync();
        public async Task<KhuyenMai?> GetByIdAsync(int id) => await _collection.Find(x => x.IDKM == id).FirstOrDefaultAsync();
        public async Task CreateAsync(KhuyenMai entity) => await _collection.InsertOneAsync(entity);
        public async Task UpdateAsync(int id, KhuyenMai entity) => await _collection.ReplaceOneAsync(x => x.IDKM == id, entity);
        public async Task DeleteAsync(int id) => await _collection.DeleteOneAsync(x => x.IDKM == id);
    }
}