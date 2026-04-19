using MongoDB.Driver;
using CarShop.Models;

namespace CarShop.Services
{
    public class LichSuDoiDiemService
    {
        private readonly IMongoCollection<LichSuDoiDiem> _collection;
        public LichSuDoiDiemService(MongoDbContext dbContext) => _collection = dbContext.LichSuDoiDiem;
        public async Task<List<LichSuDoiDiem>> GetAllAsync() => await _collection.Find(_ => true).ToListAsync();
        public async Task<LichSuDoiDiem?> GetByIdAsync(int id) => await _collection.Find(x => x.ID == id).FirstOrDefaultAsync();
        public async Task<List<LichSuDoiDiem>> GetByKhachHangIdAsync(int idKH) => await _collection.Find(x => x.IDKH == idKH).ToListAsync();
        public async Task CreateAsync(LichSuDoiDiem entity) => await _collection.InsertOneAsync(entity);
        public async Task UpdateAsync(int id, LichSuDoiDiem entity) => await _collection.ReplaceOneAsync(x => x.ID == id, entity);
        public async Task DeleteAsync(int id) => await _collection.DeleteOneAsync(x => x.ID == id);
    }
}