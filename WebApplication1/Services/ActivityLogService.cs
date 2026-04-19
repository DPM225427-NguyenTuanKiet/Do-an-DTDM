using MongoDB.Driver;
using CarShop.Models;

namespace CarShop.Services
{
    public class ActivityLogService
    {
        private readonly IMongoCollection<ActivityLog> _collection;
        public ActivityLogService(MongoDbContext dbContext) => _collection = dbContext.ActivityLog;
        public async Task<List<ActivityLog>> GetAllAsync() => await _collection.Find(_ => true).ToListAsync();
        public async Task<ActivityLog?> GetByIdAsync(int id) => await _collection.Find(x => x.IDLOG == id).FirstOrDefaultAsync();
        public async Task<List<ActivityLog>> GetByTaiKhoanIdAsync(int idTK) => await _collection.Find(x => x.IDTK == idTK).ToListAsync();
        public async Task CreateAsync(ActivityLog entity) => await _collection.InsertOneAsync(entity);
        public async Task DeleteAsync(int id) => await _collection.DeleteOneAsync(x => x.IDLOG == id);
    }
}