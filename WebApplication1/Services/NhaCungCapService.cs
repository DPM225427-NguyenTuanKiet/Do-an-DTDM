using MongoDB.Driver;
using CarShop.Models;

namespace CarShop.Services
{
    public class NhaCungCapService
    {
        private readonly IMongoCollection<NhaCungCap> _collection;
        public NhaCungCapService(MongoDbContext dbContext) => _collection = dbContext.NhaCungCap;
        public async Task<List<NhaCungCap>> GetAllAsync() => await _collection.Find(_ => true).ToListAsync();
        public async Task<NhaCungCap?> GetByIdAsync(int id) => await _collection.Find(x => x.IDNCC == id).FirstOrDefaultAsync();
        public async Task CreateAsync(NhaCungCap entity) => await _collection.InsertOneAsync(entity);
        public async Task UpdateAsync(int id, NhaCungCap entity) => await _collection.ReplaceOneAsync(x => x.IDNCC == id, entity);
        public async Task DeleteAsync(int id) => await _collection.DeleteOneAsync(x => x.IDNCC == id);
    }
}