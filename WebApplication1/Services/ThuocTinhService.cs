using MongoDB.Driver;
using CarShop.Models;

namespace CarShop.Services
{
    public class ThuocTinhService
    {
        private readonly IMongoCollection<ThuocTinh> _collection;
        public ThuocTinhService(MongoDbContext dbContext) => _collection = dbContext.ThuocTinh;
        public async Task<List<ThuocTinh>> GetAllAsync() => await _collection.Find(_ => true).ToListAsync();
        public async Task<ThuocTinh?> GetByIdAsync(int id) => await _collection.Find(x => x.IDTT == id).FirstOrDefaultAsync();
        public async Task CreateAsync(ThuocTinh entity) => await _collection.InsertOneAsync(entity);
        public async Task UpdateAsync(int id, ThuocTinh entity) => await _collection.ReplaceOneAsync(x => x.IDTT == id, entity);
        public async Task DeleteAsync(int id) => await _collection.DeleteOneAsync(x => x.IDTT == id);
    }
}