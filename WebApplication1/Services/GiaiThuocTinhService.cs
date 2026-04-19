using MongoDB.Driver;
using CarShop.Models;

namespace CarShop.Services
{
    public class GiaiThuocTinhService
    {
        private readonly IMongoCollection<GiaiThuocTinh> _collection;
        public GiaiThuocTinhService(MongoDbContext dbContext) => _collection = dbContext.GiaiThuocTinh;

        public async Task<List<GiaiThuocTinh>> GetAllAsync() => await _collection.Find(_ => true).ToListAsync();
        public async Task<GiaiThuocTinh?> GetByIdAsync(string id) => await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();
        public async Task<GiaiThuocTinh?> GetBySanPhamAndThuocTinhAsync(int idSP, int idTT) => await _collection.Find(x => x.IDSP == idSP && x.IDTT == idTT).FirstOrDefaultAsync();
        public async Task CreateAsync(GiaiThuocTinh entity) => await _collection.InsertOneAsync(entity);
        public async Task UpdateAsync(string id, GiaiThuocTinh entity) => await _collection.ReplaceOneAsync(x => x.Id == id, entity);
        public async Task DeleteAsync(string id) => await _collection.DeleteOneAsync(x => x.Id == id);
    }
}