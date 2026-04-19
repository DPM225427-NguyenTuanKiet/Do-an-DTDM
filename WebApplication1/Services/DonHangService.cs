using CarShop.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarShop.Services
{
    public class DonHangService
    {
        private readonly IMongoCollection<DonHang> _donHangCollection;

        // Constructor - sửa tên collection cho đúng với DbContext của bạn
        public DonHangService(MongoDbContext dbContext)
        {
            _donHangCollection = dbContext.DonHang; // Giả sử DbContext có property DonHang
        }

        // Lấy tất cả
        public async Task<List<DonHang>> GetAllAsync()
        {
            return await _donHangCollection.Find(_ => true).ToListAsync();
        }

        // Lấy theo ID
        public async Task<DonHang?> GetByIdAsync(int id)
        {
            return await _donHangCollection.Find(x => x.IDDH == id).FirstOrDefaultAsync();
        }

        // Lấy theo ID khách hàng
        public async Task<List<DonHang>> GetByKhachHangIdAsync(int khachHangId)
        {
            return await _donHangCollection.Find(x => x.IDKH == khachHangId).ToListAsync();
        }

        // Lấy đơn hàng mới nhất (để sinh ID)
        public async Task<DonHang?> GetLastOrderAsync()
        {
            return await _donHangCollection.Find(_ => true)
                .SortByDescending(x => x.IDDH)
                .Limit(1)
                .FirstOrDefaultAsync();
        }

        // Tạo mới
        public async Task CreateAsync(DonHang order)
        {
            await _donHangCollection.InsertOneAsync(order);
        }

        // Cập nhật
        public async Task UpdateAsync(int id, DonHang order)
        {
            await _donHangCollection.ReplaceOneAsync(x => x.IDDH == id, order);
        }

        // Cập nhật trạng thái
        public async Task UpdateStatusAsync(int id, string status)
        {
            var update = Builders<DonHang>.Update.Set(x => x.TRANGTHAI, status);
            await _donHangCollection.UpdateOneAsync(x => x.IDDH == id, update);
        }

        // Xóa
        public async Task DeleteAsync(int id)
        {
            await _donHangCollection.DeleteOneAsync(x => x.IDDH == id);
        }
    }
}