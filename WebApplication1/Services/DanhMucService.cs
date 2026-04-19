using MongoDB.Driver;
using CarShop.Models;

namespace CarShop.Services
{
    public class DanhMucService
    {
        private readonly IMongoCollection<DanhMuc> _collection;

        public DanhMucService(MongoDbContext dbContext)
        {
            _collection = dbContext.DanhMuc;
        }

        public async Task<List<DanhMuc>> GetAllAsync() =>
            await _collection.Find(_ => true).ToListAsync();

        public async Task<DanhMuc?> GetByIdAsync(int id) =>
            await _collection.Find(x => x.IDDM == id).FirstOrDefaultAsync();

        public async Task CreateAsync(DanhMuc entity) =>
            await _collection.InsertOneAsync(entity);

        public async Task UpdateAsync(int id, DanhMuc entity) =>
            await _collection.ReplaceOneAsync(x => x.IDDM == id, entity);

        public async Task DeleteAsync(int id) =>
            await _collection.DeleteOneAsync(x => x.IDDM == id);
    }
}