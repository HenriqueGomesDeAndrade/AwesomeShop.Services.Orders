using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwesomeShop.Services.Orders.Infrastructure.CacheStorage
{
    public class CacheService : ICacheService
    {
        private readonly IDistributedCache _cache;
        public CacheService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task<T> GetAsync<T>(string key)
        {
            var objectString = await _cache.GetStringAsync(key);
            if (string.IsNullOrEmpty(objectString))
            {
                return default(T);
            }

            return JsonConvert.DeserializeObject<T>(objectString);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? timeToLive = null)
        {
            var memoryCacheOptions = new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = timeToLive ?? TimeSpan.FromSeconds(3600),
                SlidingExpiration = TimeSpan.FromSeconds(1200)
            };

            var objectString = JsonConvert.SerializeObject(value);
            await _cache.SetStringAsync(key, objectString, memoryCacheOptions);
        }
    }
}
