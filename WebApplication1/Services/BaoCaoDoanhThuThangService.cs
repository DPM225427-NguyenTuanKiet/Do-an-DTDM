using MongoDB.Driver;
using CarShop.Models;

namespace CarShop.Services
{
    public class BaoCaoDoanhThuThangService
    {
        private readonly IMongoCollection<BaoCaoDoanhThuThang> _collection;

        public BaoCaoDoanhThuThangService(MongoDbContext dbContext)
        {
            _collection = dbContext.BaoCaoDoanhThuThang;
        }

        public async Task<List<BaoCaoDoanhThuThang>> GetAllAsync() =>
            await _collection.Find(_ => true).ToListAsync();

        public async Task<BaoCaoDoanhThuThang?> GetByMonthYearAsync(int month, int year) =>
            await _collection.Find(x => x.THANG == month && x.NAM == year).FirstOrDefaultAsync();

        // Lấy theo khoảng thời gian (năm, tháng)
        public async Task<List<BaoCaoDoanhThuThang>> GetByYearAsync(int year) =>
            await _collection.Find(x => x.NAM == year).ToListAsync();

        public async Task<List<BaoCaoDoanhThuThang>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var startYear = startDate.Year;
            var startMonth = startDate.Month;
            var endYear = endDate.Year;
            var endMonth = endDate.Month;
            var filter = Builders<BaoCaoDoanhThuThang>.Filter.Where(x =>
                (x.NAM > startYear || (x.NAM == startYear && x.THANG >= startMonth)) &&
                (x.NAM < endYear || (x.NAM == endYear && x.THANG <= endMonth)));
            return await _collection.Find(filter).ToListAsync();
        }

        public async Task CreateAsync(BaoCaoDoanhThuThang entity) =>
            await _collection.InsertOneAsync(entity);

        public async Task UpdateAsync(int id, BaoCaoDoanhThuThang entity) =>
            await _collection.ReplaceOneAsync(x => x.IDBCT == id, entity);

        public async Task DeleteAsync(int id) =>
            await _collection.DeleteOneAsync(x => x.IDBCT == id);
    }
}