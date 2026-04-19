using MongoDB.Driver;
using CarShop.Models;

namespace CarShop.Services
{
    public class NhanVienService
    {
        private readonly IMongoCollection<NhanVien> _collection;
        private readonly PhieuLuongService _phieuLuongService;
        public NhanVienService(MongoDbContext dbContext) => _collection = dbContext.NhanVien;
        public async Task<List<NhanVien>> GetAllAsync() => await _collection.Find(_ => true).ToListAsync();
        public async Task<NhanVien?> GetByIdAsync(int id) => await _collection.Find(x => x.IDNV == id).FirstOrDefaultAsync();
        public async Task CreateAsync(NhanVien entity) => await _collection.InsertOneAsync(entity);
        public async Task UpdateAsync(int id, NhanVien entity) => await _collection.ReplaceOneAsync(x => x.IDNV == id, entity);
        public async Task DeleteAsync(int id) => await _collection.DeleteOneAsync(x => x.IDNV == id);
        public async Task RecalculateTotalSalary(int idNV)
        {
            var nhanVien = await GetByIdAsync(idNV);
            if (nhanVien == null) return;
            var allPhieu = await _phieuLuongService.GetByNhanVienIdAsync(idNV);
            nhanVien.TONGLUONG = allPhieu.Sum(p => p.TONG_LINH ?? 0);
            await UpdateAsync(idNV, nhanVien);
        }
    }

}