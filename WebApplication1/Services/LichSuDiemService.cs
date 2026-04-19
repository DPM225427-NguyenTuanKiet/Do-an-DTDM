using MongoDB.Driver;
using CarShop.Models;

namespace CarShop.Services
{
    public class LichSuDiemService
    {
        private readonly IMongoCollection<LichSuDiem> _collection;
        public LichSuDiemService(MongoDbContext dbContext) => _collection = dbContext.LichSuDiem;
        public async Task<List<LichSuDiem>> GetAllAsync() => await _collection.Find(_ => true).ToListAsync();
        public async Task<LichSuDiem?> GetByIdAsync(int id) => await _collection.Find(x => x.ID == id).FirstOrDefaultAsync();
        public async Task<List<LichSuDiem>> GetByKhachHangIdAsync(int idKH) => await _collection.Find(x => x.IDKH == idKH).ToListAsync();
        public async Task CreateAsync(LichSuDiem entity) => await _collection.InsertOneAsync(entity);
        public async Task UpdateAsync(int id, LichSuDiem entity) => await _collection.ReplaceOneAsync(x => x.ID == id, entity);
        public async Task DeleteAsync(int id) => await _collection.DeleteOneAsync(x => x.ID == id);
    }
}