using MongoDB.Driver;
using CarShop.Models;

namespace CarShop.Services
{
    public class KhoService
    {
        private readonly IMongoCollection<Kho> _collection;
        public KhoService(MongoDbContext dbContext) => _collection = dbContext.Kho;
        public async Task<List<Kho>> GetAllAsync() => await _collection.Find(_ => true).ToListAsync();
        public async Task<Kho?> GetByIdAsync(int id) => await _collection.Find(x => x.IDKHO == id).FirstOrDefaultAsync();
        public async Task CreateAsync(Kho entity) => await _collection.InsertOneAsync(entity);
        public async Task UpdateAsync(int id, Kho entity) => await _collection.ReplaceOneAsync(x => x.IDKHO == id, entity);
        public async Task DeleteAsync(int id) => await _collection.DeleteOneAsync(x => x.IDKHO == id);
    }
}