using API.Configuration;
using API.DTO.Domain;
using API.DTO.Mappers;
using Microsoft.Extensions.Options;
using Org.BouncyCastle.Asn1.Mozilla;
using StackExchange.Redis;

namespace API.Helpers
{
    public class RedisService
    {
        private readonly IConnectionMultiplexer _redis;

        public ForSession Session { get; }
        public RedisService(IConnectionMultiplexer redis, IOptions<TLLSettings> options)
        {
            _redis = redis;
            Session = new ForSession(_redis, options);
        }
        public class ForSession
        {
            private IDatabase _db;
            public readonly IOptions<TLLSettings> _options;
            public ForSession(IConnectionMultiplexer redis, IOptions<TLLSettings> options)
            {
                _db = redis.GetDatabase(0);
                _options = options;
            }
            public async Task SetSessionAsync(int userId, string deviceId)
            {
                string key = $"session:{userId}:{deviceId}";
                await _db.HashSetAsync(key, "IsRevoked", false);

                await _db.KeyExpireAsync(key, _options.Value.SessionExpires);
            }
            public async Task RevokeSessionAsync(int userId, string deviceId)
            {
                string key = $"session:{userId}:{deviceId}";
                bool sessionExists = await _db.KeyExistsAsync(key);
                if (sessionExists== true)
                {
                   await _db.HashSetAsync(key, "IsRevoked", true);
                   await _db.KeyExpireAsync(key, _options.Value.SessionIsRevokedExpires);
                }
            }
            public async Task<SessionData> GetSessionAsync(int userId, string deviceId)
            {
                string key = $"session:{userId}:{deviceId}";
                HashEntry[] hashEntries = await _db.HashGetAllAsync(key);
                if (hashEntries.Length == 0)
                    throw new AppException("Сессия не существует");

                SessionData session = ToDomainMapper.ToSessionData(hashEntries);
       
                return session;

            }
        }

    }

    }

