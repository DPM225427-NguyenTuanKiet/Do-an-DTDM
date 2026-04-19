using MongoDB.Driver;
using CarShop.Models;

namespace CarShop.Services
{
    public class GiaKhuyenMaiService
    {
        private readonly IMongoCollection<GiaKhuyenMai> _collection;
        public GiaKhuyenMaiService(MongoDbContext dbContext) => _collection = dbContext.GiaKhuyenMai;
        public async Task<List<GiaKhuyenMai>> GetAllAsync() => await _collection.Find(_ => true).ToListAsync();
        public async Task<GiaKhuyenMai?> GetByIdAsync(int id) => await _collection.Find(x => x.IDGKM == id).FirstOrDefaultAsync();
        public async Task CreateAsync(GiaKhuyenMai entity) => await _collection.InsertOneAsync(entity);
        public async Task UpdateAsync(int id, GiaKhuyenMai entity) => await _collection.ReplaceOneAsync(x => x.IDGKM == id, entity);
        public async Task DeleteAsync(int id) => await _collection.DeleteOneAsync(x => x.IDGKM == id);

    }
}