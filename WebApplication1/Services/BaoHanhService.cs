using MongoDB.Driver;
using CarShop.Models;

namespace CarShop.Services
{
    public class BaoHanhService
    {
        private readonly IMongoCollection<BaoHanh> _collection;
        public BaoHanhService(MongoDbContext dbContext) => _collection = dbContext.BaoHanh;
        public async Task<List<BaoHanh>> GetAllAsync() => await _collection.Find(_ => true).ToListAsync();
        public async Task<BaoHanh?> GetByIdAsync(int id) => await _collection.Find(x => x.IDBH == id).FirstOrDefaultAsync();
        public async Task<List<BaoHanh>> GetBySanPhamIdAsync(int idSP) => await _collection.Find(x => x.IDSP == idSP).ToListAsync();
        public async Task<List<BaoHanh>> GetByKhachHangIdAsync(int idKH) => await _collection.Find(x => x.IDKH == idKH).ToListAsync();
        public async Task CreateAsync(BaoHanh entity) => await _collection.InsertOneAsync(entity);
        public async Task UpdateAsync(int id, BaoHanh entity) => await _collection.ReplaceOneAsync(x => x.IDBH == id, entity);
        public async Task DeleteAsync(int id) => await _collection.DeleteOneAsync(x => x.IDBH == id);
    }
}