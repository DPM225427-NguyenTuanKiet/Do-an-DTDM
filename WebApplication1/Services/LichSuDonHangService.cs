using MongoDB.Driver;
using CarShop.Models;

namespace CarShop.Services
{
    public class LichSuDonHangService
    {
        private readonly IMongoCollection<LichSuDonHang> _collection;
        public LichSuDonHangService(MongoDbContext dbContext) => _collection = dbContext.LichSuDonHang;
        public async Task<List<LichSuDonHang>> GetAllAsync() => await _collection.Find(_ => true).ToListAsync();
        public async Task<LichSuDonHang?> GetByIdAsync(int id) => await _collection.Find(x => x.IDLS == id).FirstOrDefaultAsync();
        public async Task<List<LichSuDonHang>> GetByDonHangIdAsync(int idDH) => await _collection.Find(x => x.IDDH == idDH).ToListAsync();
        public async Task CreateAsync(LichSuDonHang entity) => await _collection.InsertOneAsync(entity);
        public async Task DeleteAsync(int id) => await _collection.DeleteOneAsync(x => x.IDLS == id);
    }
}