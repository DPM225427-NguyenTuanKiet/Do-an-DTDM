using MongoDB.Driver;
using CarShop.Models;

namespace CarShop.Services
{
    public class HinhAnhService
    {

        private readonly IMongoCollection<HinhAnh> _collection;
        public HinhAnhService(MongoDbContext dbContext) => _collection = dbContext.HinhAnh;
        public async Task<List<HinhAnh>> GetAllAsync() => await _collection.Find(_ => true).ToListAsync();
        public async Task<HinhAnh?> GetByIdAsync(int id) => await _collection.Find(x => x.IDHINHANH == id).FirstOrDefaultAsync();
        public async Task<List<HinhAnh>> GetBySanPhamIdAsync(int idSP) => await _collection.Find(x => x.IDSP == idSP).ToListAsync();
        public async Task CreateAsync(HinhAnh entity) => await _collection.InsertOneAsync(entity);
        public async Task UpdateAsync(int id, HinhAnh entity) => await _collection.ReplaceOneAsync(x => x.IDHINHANH == id, entity);
        public async Task DeleteAsync(int id) => await _collection.DeleteOneAsync(x => x.IDHINHANH == id);
    }
}