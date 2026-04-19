using MongoDB.Driver;
using CarShop.Models;

namespace CarShop.Services
{
    public class SanPhamService
    {
        private readonly IMongoCollection<SanPham> _collection;

        public SanPhamService(MongoDbContext dbContext)
        {
            _collection = dbContext.SanPham;
        }

        public async Task<List<SanPham>> GetAllAsync() =>
            await _collection.Find(_ => true).ToListAsync();

        public async Task<SanPham?> GetByIdAsync(int id) =>
            await _collection.Find(x => x.IDSP == id).FirstOrDefaultAsync();

        public async Task CreateAsync(SanPham entity) =>
            await _collection.InsertOneAsync(entity);

        public async Task UpdateAsync(int id, SanPham entity) =>
            await _collection.ReplaceOneAsync(x => x.IDSP == id, entity);
        public async Task<List<SanPham>> GetNewProductsAsync(int take)
        {
            var all = await GetAllAsync();
            return all.Where(p => p.TRANGTHAI).OrderByDescending(p => p.NGAYTAO).Take(take).ToList();
        }

        public async Task<List<SanPham>> GetSaleProductsAsync(int take)
        {
            var all = await GetAllAsync();
            // Giả sử bạn có bảng GiaKhuyenMai để biết sản phẩm đang khuyến mãi
            // Ở đây tạm lấy sản phẩm bất kỳ (có thể lọc theo logic riêng)
            // Nếu có service GiaKhuyenMai, hãy inject và lấy danh sách IDSP đang KM
            var saleProductIds = new List<int>(); // cần lấy thực tế
            return all.Where(p => p.TRANGTHAI && saleProductIds.Contains(p.IDSP)).Take(take).ToList();
        }

        public async Task DeleteAsync(int id) =>
            await _collection.DeleteOneAsync(x => x.IDSP == id);
    }
}