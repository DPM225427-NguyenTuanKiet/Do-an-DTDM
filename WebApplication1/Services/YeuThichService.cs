using MongoDB.Driver;
using CarShop.Models;

namespace CarShop.Services
{
    public class YeuThichService
    {
        private readonly IMongoCollection<YeuThich> _collection;
        public YeuThichService(MongoDbContext dbContext) => _collection = dbContext.YeuThich;
        public async Task<List<YeuThich>> GetAllAsync() => await _collection.Find(_ => true).ToListAsync();
        public async Task<YeuThich?> GetByIdAsync(int id) => await _collection.Find(x => x.IDYeuThich == id).FirstOrDefaultAsync();
        public async Task<List<YeuThich>> GetByKhachHangIdAsync(int idKH) => await _collection.Find(x => x.IDKH == idKH).ToListAsync();
        public async Task<bool> IsFavouriteAsync(int idKH, int idSP) => await _collection.Find(x => x.IDKH == idKH && x.IDSP == idSP).AnyAsync();
        public async Task CreateAsync(YeuThich entity) => await _collection.InsertOneAsync(entity);
        public async Task DeleteByKhachHangAndSanPhamAsync(int idKH, int idSP) => await _collection.DeleteOneAsync(x => x.IDKH == idKH && x.IDSP == idSP);
    }
}