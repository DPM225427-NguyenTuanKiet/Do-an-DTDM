using MongoDB.Driver;
using CarShop.Models;

namespace CarShop.Services
{
    public class TaiKhoanService
    {
        private readonly IMongoCollection<TaiKhoan> _collection;

        public TaiKhoanService(MongoDbContext dbContext)
        {
            _collection = dbContext.TaiKhoan;
        }

        public async Task<List<TaiKhoan>> GetAllAsync() =>
            await _collection.Find(_ => true).ToListAsync();

        public async Task<TaiKhoan?> GetByIdAsync(int id) =>
            await _collection.Find(x => x.IDTK == id).FirstOrDefaultAsync();

        public async Task<TaiKhoan?> GetByUsernameAsync(string username) =>
            await _collection.Find(x => x.TENDANGNHAP == username).FirstOrDefaultAsync();
        public async Task<TaiKhoan?> GetByEmailAsync(string email) =>
            await _collection.Find(x => x.EMAIL == email).FirstOrDefaultAsync();

        public async Task CreateAsync(TaiKhoan entity) =>
            await _collection.InsertOneAsync(entity);

        public async Task UpdateAsync(int id, TaiKhoan entity) =>
            await _collection.ReplaceOneAsync(x => x.IDTK == id, entity);

        public async Task DeleteAsync(int id) =>
            await _collection.DeleteOneAsync(x => x.IDTK == id);

    }
}