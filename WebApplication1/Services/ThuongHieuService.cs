using MongoDB.Driver;
using CarShop.Models;

namespace CarShop.Services
{
    public class ThuongHieuService
    {
        private readonly IMongoCollection<ThuongHieu> _collection;

        public ThuongHieuService(MongoDbContext dbContext)
        {
            _collection = dbContext.ThuongHieu;
        }

        public async Task<List<ThuongHieu>> GetAllAsync() =>
            await _collection.Find(_ => true).ToListAsync();

        public async Task<ThuongHieu?> GetByIdAsync(int id) =>
            await _collection.Find(x => x.IDTH == id).FirstOrDefaultAsync();

        public async Task CreateAsync(ThuongHieu entity) =>
            await _collection.InsertOneAsync(entity);

        public async Task UpdateAsync(int id, ThuongHieu entity) =>
            await _collection.ReplaceOneAsync(x => x.IDTH == id, entity);

        public async Task DeleteAsync(int id) =>
            await _collection.DeleteOneAsync(x => x.IDTH == id);
    }
}