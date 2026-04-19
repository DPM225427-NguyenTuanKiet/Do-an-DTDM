using MongoDB.Driver;
using CarShop.Models;

namespace CarShop.Services
{
    public class ThongSoKyThuatService
    {
        private readonly IMongoCollection<ThongSoKyThuat> _collection;

        public ThongSoKyThuatService(MongoDbContext dbContext)
        {
            _collection = dbContext.ThongSoKyThuat;
        }

        public async Task<List<ThongSoKyThuat>> GetAllAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        public async Task<ThongSoKyThuat?> GetByIdAsync(int id)
        {
            return await _collection.Find(x => x.IDTS == id).FirstOrDefaultAsync();
        }

        public async Task<ThongSoKyThuat?> GetBySanPhamIdAsync(int idSP)
        {
            return await _collection.Find(x => x.IDSP == idSP).FirstOrDefaultAsync();
        }

        public async Task CreateAsync(ThongSoKyThuat entity)
        {
            await _collection.InsertOneAsync(entity);
        }

        public async Task UpdateAsync(int id, ThongSoKyThuat entity)
        {
            await _collection.ReplaceOneAsync(x => x.IDTS == id, entity);
        }

        public async Task DeleteAsync(int id)
        {
            await _collection.DeleteOneAsync(x => x.IDTS == id);
        }

        public async Task<bool> ExistsBySanPhamIdAsync(int idSP)
        {
            var count = await _collection.CountDocumentsAsync(x => x.IDSP == idSP);
            return count > 0;
        }

        public async Task UpsertBySanPhamIdAsync(int idSP, ThongSoKyThuat entity)
        {
            var existing = await GetBySanPhamIdAsync(idSP);
            if (existing != null)
            {
                entity.IDTS = existing.IDTS; // giữ nguyên ID cũ
                await _collection.ReplaceOneAsync(x => x.IDSP == idSP, entity);
            }
            else
            {
                var all = await GetAllAsync();
                var maxId = all.Any() ? all.Max(x => x.IDTS) : 0;
                entity.IDTS = maxId + 1;
                entity.IDSP = idSP;
                await _collection.InsertOneAsync(entity);
            }
        }
    }
}