using MongoDB.Driver;
using CarShop.Models;

namespace CarShop.Services
{
    public class KhachHangService
    {
        private readonly IMongoCollection<KhachHang> _collection;

        public KhachHangService(MongoDbContext dbContext)
        {
            _collection = dbContext.KhachHang;
        }

        public async Task<List<KhachHang>> GetAllAsync() =>
            await _collection.Find(_ => true).ToListAsync();

        public async Task<KhachHang?> GetByIdAsync(int id) =>
            await _collection.Find(x => x.IDKH == id).FirstOrDefaultAsync();

        public async Task<KhachHang?> GetByEmailAsync(string email) =>
            await _collection.Find(x => x.EMAIL == email).FirstOrDefaultAsync();

        public async Task CreateAsync(KhachHang entity) =>
            await _collection.InsertOneAsync(entity);

        public async Task UpdateAsync(int id, KhachHang entity) =>
            await _collection.ReplaceOneAsync(x => x.IDKH == id, entity);
        public async Task DeleteAsync(int id) => await _collection.DeleteOneAsync(x => x.IDKH == id);
        public async Task<KhachHang?> GetByTaiKhoanIdAsync(int idTK)
        {
            return await _collection.Find(kh => kh.IDTK == idTK).FirstOrDefaultAsync();
        }
    }
}