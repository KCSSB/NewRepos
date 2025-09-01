using API.Configuration;
using API.Constants;
using API.DTO.Domain;
using API.DTO.Mappers;
using API.Exceptions;
using Microsoft.Extensions.Options;
using API.Exceptions.Context;
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
            public readonly ErrorContextCreator _errCreator;
            public ForSession(IConnectionMultiplexer redis, IOptions<TLLSettings> options)
            {
                _db = redis.GetDatabase(0);
                _options = options;
                _errCreator = new ErrorContextCreator(ServiceName.RedisService);
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
                if (sessionExists == false)
                    throw new AppException(_errCreator.Unauthorized($"Сессия не существует: userId: {userId} deviceId: {deviceId}"));
                
                    var tasks = new Task[]
                    {
                   _db.HashSetAsync(key, "IsRevoked", true),
                   _db.KeyExpireAsync(key, _options.Value.SessionIsRevokedExpires)
                    };
                await Task.WhenAll(tasks);
            }
            public async Task<SessionData> GetSessionAsync(int userId, string deviceId)
            {
                string key = $"session:{userId}:{deviceId}";
                HashEntry[] hashEntries = await _db.HashGetAllAsync(key);
                if (hashEntries.Length == 0)
                    throw new AppException(_errCreator.Unauthorized($"Сессия не существует: userId: {userId} deviceId: {deviceId}"));
                SessionData session = ToDomainMapper.ToSessionData(hashEntries);
                return session;

            }
        }

    }

    }

