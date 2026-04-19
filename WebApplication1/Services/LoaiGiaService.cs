using CarShop.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarShop.Services
{
    public class LoaiGiaService
    {
        private readonly IMongoCollection<LoaiGia> _loaiGias;

        public LoaiGiaService(MongoDbContext dbContext)
        {
            _loaiGias = dbContext.LoaiGia; // giả sử DbContext có property LoaiGia
        }

        public async Task<List<LoaiGia>> GetAllAsync()
        {
            return await _loaiGias.Find(_ => true).ToListAsync();
        }

        public async Task<LoaiGia?> GetByIdAsync(int id)
        {
            return await _loaiGias.Find(x => x.IDLG == id).FirstOrDefaultAsync();
        }

        public async Task<LoaiGia?> GetByDiemToiThieuAsync(int diemHienTai)
        {
            return await _loaiGias
                .Find(lg => lg.DIEM_TOITHIEU <= diemHienTai)
                .SortByDescending(lg => lg.DIEM_TOITHIEU)
                .FirstOrDefaultAsync();
        }

        public async Task CreateAsync(LoaiGia entity) => await _loaiGias.InsertOneAsync(entity);
        public async Task UpdateAsync(int id, LoaiGia entity) => await _loaiGias.ReplaceOneAsync(x => x.IDLG == id, entity);
        public async Task DeleteAsync(int id) => await _loaiGias.DeleteOneAsync(x => x.IDLG == id);
    }
}