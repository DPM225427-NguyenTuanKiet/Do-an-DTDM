using MongoDB.Driver;
using CarShop.Models;

namespace CarShop.Services
{
    public class BinhLuanService
    {
        private readonly IMongoCollection<BinhLuan> _collection;
        public BinhLuanService(MongoDbContext dbContext) => _collection = dbContext.BinhLuan;
        public async Task<List<BinhLuan>> GetAllAsync() => await _collection.Find(_ => true).ToListAsync();
        public async Task<BinhLuan?> GetByIdAsync(int id) => await _collection.Find(x => x.IDBL == id).FirstOrDefaultAsync();
        public async Task<List<BinhLuan>> GetBySanPhamIdAsync(int idSP) => await _collection.Find(x => x.IDSP == idSP).ToListAsync();
        public async Task<List<BinhLuan>> GetByTaiKhoanIdAsync(int idTK) => await _collection.Find(x => x.IDTK == idTK).ToListAsync();
        public async Task CreateAsync(BinhLuan entity) => await _collection.InsertOneAsync(entity);
        public async Task UpdateAsync(int id, BinhLuan entity) => await _collection.ReplaceOneAsync(x => x.IDBL == id, entity);
        public async Task DeleteAsync(int id) => await _collection.DeleteOneAsync(x => x.IDBL == id);
    }
}