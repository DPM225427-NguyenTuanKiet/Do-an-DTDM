using MongoDB.Driver;
using CarShop.Models;

namespace CarShop.Services
{
    public class HoaDonService
    {
        private readonly IMongoCollection<HoaDon> _collection;
        public HoaDonService(MongoDbContext dbContext) => _collection = dbContext.HoaDon;
        public async Task<List<HoaDon>> GetAllAsync() => await _collection.Find(_ => true).ToListAsync();
        public async Task<HoaDon?> GetByIdAsync(int id) => await _collection.Find(x => x.IDHD == id).FirstOrDefaultAsync();
        public async Task<List<HoaDon>> GetByDonHangIdAsync(int idDH) => await _collection.Find(x => x.IDDH == idDH).ToListAsync();
        public async Task CreateAsync(HoaDon entity) => await _collection.InsertOneAsync(entity);
        public async Task UpdateAsync(int id, HoaDon entity) => await _collection.ReplaceOneAsync(x => x.IDHD == id, entity);
        public async Task DeleteAsync(int id) => await _collection.DeleteOneAsync(x => x.IDHD == id);
    }
}