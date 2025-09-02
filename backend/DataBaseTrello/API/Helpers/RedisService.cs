using API.Configuration;
using API.Constants;
using API.DTO.Domain;
using API.DTO.Mappers;
using API.Exceptions;
using Microsoft.Extensions.Options;
using API.Exceptions.Context;
using StackExchange.Redis;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

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
            public readonly ErrorContextCreator _errCreator;
            public ForSession(IConnectionMultiplexer redis, IOptions<TLLSettings> options)
            {
                _db = redis.GetDatabase(0);
                _options = options;
                _errCreator = new ErrorContextCreator(ServiceName.RedisService);
            }
            public async Task SetSessionAsync(string refreshToken,int userId, string deviceId)
            {
                string key = $"session:{userId}:{deviceId}";
                var entries = new HashEntry[]
                {
                new HashEntry("IsRevoked", false),
                new HashEntry("HashedToken", refreshToken)
                };
            await _db.HashSetAsync(key, entries);

            await _db.KeyExpireAsync(key, _options.Value.SessionExpires);
                
            }
            public async Task<bool> RevokeSessionAsync(int userId, string deviceId)
            {
                string key = $"session:{userId}:{deviceId}";
                bool sessionExists = await _db.KeyExistsAsync(key);
                if (sessionExists == false)
                    return false;
                
                    var tasks = new Task[]
                    {
                   _db.HashSetAsync(key, "IsRevoked", true),
                   _db.KeyExpireAsync(key, _options.Value.SessionIsRevokedExpires)
                    };
                await Task.WhenAll(tasks);
                return true;
            }
            public async Task<SessionData?> GetSessionAsync(int userId, string deviceId)
            {
                string key = $"session:{userId}:{deviceId}";
                HashEntry[] hashEntries = await _db.HashGetAllAsync(key);
                if (hashEntries.Length == 0)
                    return null;
                SessionData session = ToDomainMapper.ToSessionData(hashEntries);
                return session;

            }
        }

    }

    }

