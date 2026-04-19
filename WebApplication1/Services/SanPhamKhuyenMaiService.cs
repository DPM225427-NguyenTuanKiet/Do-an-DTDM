using MongoDB.Driver;
using CarShop.Models;

namespace CarShop.Services
{
    public class SanPhamKhuyenMaiService
    {
        private readonly IMongoCollection<SanPhamKhuyenMai> _collection;
        public SanPhamKhuyenMaiService(MongoDbContext dbContext)
        {
            _collection = dbContext.SanPhamKhuyenMai;
        }

        public async Task<List<SanPhamKhuyenMai>> GetAllAsync() =>
            await _collection.Find(_ => true).ToListAsync();

        public async Task<SanPhamKhuyenMai?> GetByIdAsync(string id) =>
            await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task<SanPhamKhuyenMai?> GetBySanPhamAndKhuyenMaiAsync(int idSP, int idKM) =>
            await _collection.Find(x => x.IDSP == idSP && x.IDKM == idKM).FirstOrDefaultAsync();

        public async Task CreateAsync(SanPhamKhuyenMai entity) =>
            await _collection.InsertOneAsync(entity);

        public async Task UpdateAsync(string id, SanPhamKhuyenMai entity) =>
            await _collection.ReplaceOneAsync(x => x.Id == id, entity);

        public async Task DeleteAsync(string id) =>
            await _collection.DeleteOneAsync(x => x.Id == id);
    }
}