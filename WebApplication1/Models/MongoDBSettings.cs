using CarShop.Models;
using MongoDB.Driver;

namespace WebApplication1.Models
{
    public class MongoDBSettings
    {
        public string ConnectionString { get; set; } = null!;
        public string DatabaseName { get; set; } = null!;
        
    }
}
