using MongoDB.Driver;
using CarShop.Models;

namespace CarShop.Services
{
    public class BaoCaoDoanhThuNgayService
    {
        private readonly IMongoCollection<BaoCaoDoanhThuNgay> _collection;

        public BaoCaoDoanhThuNgayService(MongoDbContext dbContext)
        {
            _collection = dbContext.BaoCaoDoanhThuNgay;
        }

        public async Task<List<BaoCaoDoanhThuNgay>> GetAllAsync() =>
            await _collection.Find(_ => true).ToListAsync();

        public async Task<BaoCaoDoanhThuNgay?> GetByDateAsync(DateTime date) =>
            await _collection.Find(x => x.NGAY.Date == date.Date).FirstOrDefaultAsync();

        // Thêm phương thức lấy theo khoảng thời gian
        public async Task<List<BaoCaoDoanhThuNgay>> GetByDateRangeAsync(DateTime startDate, DateTime endDate) =>
            await _collection.Find(x => x.NGAY >= startDate.Date && x.NGAY <= endDate.Date).ToListAsync();

        public async Task CreateAsync(BaoCaoDoanhThuNgay entity) =>
            await _collection.InsertOneAsync(entity);

        public async Task UpdateAsync(int id, BaoCaoDoanhThuNgay entity) =>
            await _collection.ReplaceOneAsync(x => x.IDBC == id, entity);

        public async Task DeleteAsync(int id) =>
            await _collection.DeleteOneAsync(x => x.IDBC == id);
    }
}