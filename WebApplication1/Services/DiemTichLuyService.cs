using MongoDB.Driver;
using CarShop.Models;

namespace CarShop.Services
{
    public class DiemTichLuyService
    {
        private readonly IMongoCollection<DiemTichLuy> _collection;
        public DiemTichLuyService(MongoDbContext dbContext) => _collection = dbContext.DiemTichLuy;

        public async Task<List<DiemTichLuy>> GetAllAsync() => await _collection.Find(_ => true).ToListAsync();
        public async Task<DiemTichLuy?> GetByKhachHangIdAsync(int id) => await _collection.Find(x => x.IDKH == id).FirstOrDefaultAsync();
        public async Task CreateAsync(DiemTichLuy entity) => await _collection.InsertOneAsync(entity);
        public async Task UpdateAsync(int id, DiemTichLuy entity) => await _collection.ReplaceOneAsync(x => x.IDKH == id, entity);
        public async Task DeleteAsync(int id) => await _collection.DeleteOneAsync(x => x.IDKH == id);
    }
}