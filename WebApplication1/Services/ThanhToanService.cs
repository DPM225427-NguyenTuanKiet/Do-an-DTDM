using MongoDB.Driver;
using CarShop.Models;

namespace CarShop.Services
{
    public class ThanhToanService
    {
        private readonly IMongoCollection<ThanhToan> _collection;
        public ThanhToanService(MongoDbContext dbContext) => _collection = dbContext.ThanhToan;
        public async Task<List<ThanhToan>> GetAllAsync() => await _collection.Find(_ => true).ToListAsync();
        public async Task<ThanhToan?> GetByIdAsync(int id) => await _collection.Find(x => x.IDTT == id).FirstOrDefaultAsync();
        public async Task<List<ThanhToan>> GetByDonHangIdAsync(int idDH) => await _collection.Find(x => x.IDDH == idDH).ToListAsync();
        public async Task CreateAsync(ThanhToan entity) => await _collection.InsertOneAsync(entity);
        public async Task UpdateAsync(int id, ThanhToan entity) => await _collection.ReplaceOneAsync(x => x.IDTT == id, entity);
        public async Task DeleteAsync(int id) => await _collection.DeleteOneAsync(x => x.IDTT == id);
    }
}