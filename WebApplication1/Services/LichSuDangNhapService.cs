using MongoDB.Driver;
using CarShop.Models;

namespace CarShop.Services
{
    public class LichSuDangNhapService
    {
        private readonly IMongoCollection<LichSuDangNhap> _collection;
        public LichSuDangNhapService(MongoDbContext dbContext) => _collection = dbContext.LichSuDangNhap;
        public async Task<List<LichSuDangNhap>> GetAllAsync() => await _collection.Find(_ => true).ToListAsync();
        public async Task<LichSuDangNhap?> GetByIdAsync(int id) => await _collection.Find(x => x.IDLS == id).FirstOrDefaultAsync();
        public async Task<List<LichSuDangNhap>> GetByTaiKhoanIdAsync(int idTK) => await _collection.Find(x => x.IDTK == idTK).ToListAsync();
        public async Task CreateAsync(LichSuDangNhap entity) => await _collection.InsertOneAsync(entity);
        public async Task DeleteAsync(int id) => await _collection.DeleteOneAsync(x => x.IDLS == id);
    }
}