using MongoDB.Driver;
using CarShop.Models;

namespace CarShop.Services
{
    public class PhieuNhapService
    {
        private readonly IMongoCollection<PhieuNhap> _collection;
        public PhieuNhapService(MongoDbContext dbContext) => _collection = dbContext.PhieuNhap;
        public async Task<List<PhieuNhap>> GetAllAsync() => await _collection.Find(_ => true).ToListAsync();
        public async Task<PhieuNhap?> GetByIdAsync(int id) => await _collection.Find(x => x.IDPN == id).FirstOrDefaultAsync();
        public async Task CreateAsync(PhieuNhap entity) => await _collection.InsertOneAsync(entity);
        public async Task UpdateAsync(int id, PhieuNhap entity) => await _collection.ReplaceOneAsync(x => x.IDPN == id, entity);
        public async Task DeleteAsync(int id) => await _collection.DeleteOneAsync(x => x.IDPN == id);
    }
}