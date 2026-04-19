using MongoDB.Driver;
using CarShop.Models;

namespace CarShop.Services
{
    public class TonKhoService
    {
        private readonly IMongoCollection<TonKho> _collection;
        public TonKhoService(MongoDbContext dbContext) => _collection = dbContext.TonKho;
        public async Task<List<TonKho>> GetAllAsync() => await _collection.Find(_ => true).ToListAsync();
        public async Task<TonKho?> GetByIdAsync(string id) => await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();
        public async Task<TonKho?> GetByKhoAndSanPhamAsync(int idKho, int idSP) => await _collection.Find(x => x.IDKHO == idKho && x.IDSP == idSP).FirstOrDefaultAsync();
        public async Task<List<TonKho>> GetByKhoIdAsync(int idKho) => await _collection.Find(x => x.IDKHO == idKho).ToListAsync();
        public async Task CreateAsync(TonKho entity) => await _collection.InsertOneAsync(entity);
        public async Task UpdateAsync(string id, TonKho entity) => await _collection.ReplaceOneAsync(x => x.Id == id, entity);
        public async Task DeleteAsync(string id) => await _collection.DeleteOneAsync(x => x.Id == id);
        public async Task UpdateQuantityAsync(int idKho, int idSP, int newQuantity)
        {
            var existing = await GetByKhoAndSanPhamAsync(idKho, idSP);
            if (existing != null)
            {
                existing.SOLUONGTON = newQuantity;
                await UpdateAsync(existing.Id!, existing);
            }
            else
            {
                await CreateAsync(new TonKho { IDKHO = idKho, IDSP = idSP, SOLUONGTON = newQuantity });
            }
        }
    }
}