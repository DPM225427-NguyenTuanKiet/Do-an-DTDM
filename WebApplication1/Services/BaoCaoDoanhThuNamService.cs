using MongoDB.Driver;
using CarShop.Models;

namespace CarShop.Services
{
    public class BaoCaoDoanhThuNamService
    {
        private readonly IMongoCollection<BaoCaoDoanhThuNam> _collection;

        public BaoCaoDoanhThuNamService(MongoDbContext dbContext)
        {
            _collection = dbContext.BaoCaoDoanhThuNam;
        }

        public async Task<List<BaoCaoDoanhThuNam>> GetAllAsync() =>
            await _collection.Find(_ => true).ToListAsync();

        public async Task<BaoCaoDoanhThuNam?> GetByYearAsync(int year) =>
            await _collection.Find(x => x.NAM == year).FirstOrDefaultAsync();

        public async Task<List<BaoCaoDoanhThuNam>> GetByDateRangeAsync(int startYear, int endYear) =>
            await _collection.Find(x => x.NAM >= startYear && x.NAM <= endYear).ToListAsync();

        public async Task CreateAsync(BaoCaoDoanhThuNam entity) =>
            await _collection.InsertOneAsync(entity);

        public async Task UpdateAsync(int id, BaoCaoDoanhThuNam entity) =>
            await _collection.ReplaceOneAsync(x => x.IDBCN == id, entity);

        public async Task DeleteAsync(int id) =>
            await _collection.DeleteOneAsync(x => x.IDBCN == id);
    }
}