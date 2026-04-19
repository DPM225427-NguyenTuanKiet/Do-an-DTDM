using MongoDB.Driver;
using CarShop.Models;

namespace CarShop.Services
{
    public class VoucherService
    {
        private readonly IMongoCollection<Voucher> _collection;
        public VoucherService(MongoDbContext dbContext) => _collection = dbContext.Voucher;
        public async Task<List<Voucher>> GetAllAsync() => await _collection.Find(_ => true).ToListAsync();
        public async Task<Voucher?> GetByIdAsync(int id) => await _collection.Find(x => x.IDV == id).FirstOrDefaultAsync();
        public async Task CreateAsync(Voucher entity) => await _collection.InsertOneAsync(entity);
        public async Task UpdateAsync(int id, Voucher entity) => await _collection.ReplaceOneAsync(x => x.IDV == id, entity);
        public async Task DeleteAsync(int id) => await _collection.DeleteOneAsync(x => x.IDV == id);
        public async Task<Voucher?> GetByCodeAsync(string code)
        {
            return await _collection.Find(v => v.MAVOUCHER == code).FirstOrDefaultAsync();
        }
    }
}