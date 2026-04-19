// Extensions/SessionExtensions.cs
using System.Text.Json;
using CarShop.Models;
namespace CarShop.Extensions
{
    public static class SessionExtensions
    {
        public static void SetObject<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }
        public static T? GetObject<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default : JsonSerializer.Deserialize<T>(value);
        }
        public static void SetDecimal(this ISession session, string key, decimal value)
        {
            session.SetString(key, value.ToString());
        }

        public static decimal? GetDecimal(this ISession session, string key)
        {
            var value = session.GetString(key);
            if (decimal.TryParse(value, out var result))
                return result;
            return null;
        }
    }
}