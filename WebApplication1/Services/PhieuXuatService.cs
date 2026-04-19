using MongoDB.Driver;
using CarShop.Models;

namespace CarShop.Services
{
    public class PhieuXuatService
    {
        private readonly IMongoCollection<PhieuXuat> _collection;
        public PhieuXuatService(MongoDbContext dbContext) => _collection = dbContext.PhieuXuat;
        public async Task<List<PhieuXuat>> GetAllAsync() => await _collection.Find(_ => true).ToListAsync();
        public async Task<PhieuXuat?> GetByIdAsync(int id) => await _collection.Find(x => x.IDPX == id).FirstOrDefaultAsync();
        public async Task CreateAsync(PhieuXuat entity) => await _collection.InsertOneAsync(entity);
        public async Task UpdateAsync(int id, PhieuXuat entity) => await _collection.ReplaceOneAsync(x => x.IDPX == id, entity);
        public async Task DeleteAsync(int id) => await _collection.DeleteOneAsync(x => x.IDPX == id);
    }
}