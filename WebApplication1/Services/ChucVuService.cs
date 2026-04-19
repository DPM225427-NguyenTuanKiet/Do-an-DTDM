using MongoDB.Driver;
using CarShop.Models;

namespace CarShop.Services
{
    public class ChucVuService
    {
        private readonly IMongoCollection<ChucVu> _collection;
        public ChucVuService(MongoDbContext dbContext) => _collection = dbContext.ChucVu;
        public async Task<List<ChucVu>> GetAllAsync() => await _collection.Find(_ => true).ToListAsync();
        public async Task<ChucVu?> GetByIdAsync(int id) => await _collection.Find(x => x.IDCV == id).FirstOrDefaultAsync();
        public async Task CreateAsync(ChucVu entity) => await _collection.InsertOneAsync(entity);
        public async Task UpdateAsync(int id, ChucVu entity) => await _collection.ReplaceOneAsync(x => x.IDCV == id, entity);
        public async Task DeleteAsync(int id) => await _collection.DeleteOneAsync(x => x.IDCV == id);
    }
}