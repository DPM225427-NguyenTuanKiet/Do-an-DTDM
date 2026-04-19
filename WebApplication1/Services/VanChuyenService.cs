using MongoDB.Driver;
using CarShop.Models;

namespace CarShop.Services
{
    public class VanChuyenService
    {
        private readonly IMongoCollection<VanChuyen> _collection;
        public VanChuyenService(MongoDbContext dbContext) => _collection = dbContext.VanChuyen;

        public async Task<List<VanChuyen>> GetAllAsync() => await _collection.Find(_ => true).ToListAsync();
        public async Task<VanChuyen?> GetByIdAsync(int id) => await _collection.Find(x => x.IDVC == id).FirstOrDefaultAsync();
        public async Task<VanChuyen?> GetByDonHangIdAsync(int idDH) => await _collection.Find(x => x.IDDH == idDH).FirstOrDefaultAsync();
        public async Task<List<VanChuyen>> GetByIdsAsync(List<int> orderIds)
        {
            if (orderIds == null || !orderIds.Any())
                return new List<VanChuyen>();
            return await _collection.Find(v => orderIds.Contains(v.IDDH)).ToListAsync();
        }

        public async Task CreateAsync(VanChuyen entity) => await _collection.InsertOneAsync(entity);
        public async Task UpdateAsync(int id, VanChuyen entity) => await _collection.ReplaceOneAsync(x => x.IDVC == id, entity);
        public async Task DeleteAsync(int id) => await _collection.DeleteOneAsync(x => x.IDVC == id);
    }
}