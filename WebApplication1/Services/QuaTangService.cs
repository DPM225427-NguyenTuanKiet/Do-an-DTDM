using MongoDB.Driver;
using CarShop.Models;

namespace CarShop.Services
{
    public class QuaTangService
    {
        private readonly IMongoCollection<QuaTang> _collection;
        public QuaTangService(MongoDbContext dbContext) => _collection = dbContext.QuaTang;
        public async Task<List<QuaTang>> GetAllAsync() => await _collection.Find(_ => true).ToListAsync();
        public async Task<QuaTang?> GetByIdAsync(int id) => await _collection.Find(x => x.IDQT == id).FirstOrDefaultAsync();
        public async Task CreateAsync(QuaTang entity) => await _collection.InsertOneAsync(entity);
        public async Task UpdateAsync(int id, QuaTang entity) => await _collection.ReplaceOneAsync(x => x.IDQT == id, entity);
        public async Task DeleteAsync(int id) => await _collection.DeleteOneAsync(x => x.IDQT == id);
    }
}