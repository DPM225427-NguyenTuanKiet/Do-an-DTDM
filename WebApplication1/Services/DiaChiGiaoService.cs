using MongoDB.Driver;
using CarShop.Models;

namespace CarShop.Services
{
    public class DiaChiGiaoService
    {
        private readonly IMongoCollection<DiaChiGiao> _collection;
        public DiaChiGiaoService(MongoDbContext dbContext) => _collection = dbContext.DiaChiGiao;
        public async Task<List<DiaChiGiao>> GetAllAsync() => await _collection.Find(_ => true).ToListAsync();
        public async Task<DiaChiGiao?> GetByIdAsync(int id) => await _collection.Find(x => x.IDDC == id).FirstOrDefaultAsync();
        public async Task<List<DiaChiGiao>> GetByKhachHangIdAsync(int idKH) => await _collection.Find(x => x.IDKH == idKH).ToListAsync();
        public async Task CreateAsync(DiaChiGiao entity) => await _collection.InsertOneAsync(entity);
        public async Task UpdateAsync(int id, DiaChiGiao entity) => await _collection.ReplaceOneAsync(x => x.IDDC == id, entity);
        public async Task DeleteAsync(int id) => await _collection.DeleteOneAsync(x => x.IDDC == id);
    }
}