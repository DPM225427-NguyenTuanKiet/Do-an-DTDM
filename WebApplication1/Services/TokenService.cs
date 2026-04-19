using CarShop.Models;
using MongoDB.Driver;

namespace CarShop.Services
{
    public class TokenService
    {
        private readonly IMongoCollection<Token> _collection;
        public TokenService(MongoDbContext dbContext) => _collection = dbContext.Token;
        public async Task<List<Token>> GetAllAsync() => await _collection.Find(_ => true).ToListAsync();
        public async Task<Token?> GetByIdAsync(int id) => await _collection.Find(x => x.IDTOKEN == id).FirstOrDefaultAsync();
        public async Task<Token?> GetByTokenStringAsync(string token) => await _collection.Find(x => x.TOKEN == token).FirstOrDefaultAsync();
        public async Task CreateAsync(Token entity) => await _collection.InsertOneAsync(entity);
        public async Task UpdateAsync(int id, Token entity) => await _collection.ReplaceOneAsync(x => x.IDTOKEN == id, entity);
        public async Task DeleteAsync(int id) => await _collection.DeleteOneAsync(x => x.IDTOKEN == id);
        public async Task DeleteByTokenStringAsync(string token) => await _collection.DeleteOneAsync(x => x.TOKEN == token);
    }
}