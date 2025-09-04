using API.Configuration;
using API.Constants;
using API.DTO.Domain;
using API.DTO.Mappers;
using API.Exceptions;
using Microsoft.Extensions.Options;
using API.Exceptions.Context;
using StackExchange.Redis;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Mvc;

namespace API.Helpers
{
    public class RedisService
    {
        private readonly Lazy<IConnectionMultiplexer> _lazyConnection;

        public ForSession Session { get; }
        public RedisService(Lazy<IConnectionMultiplexer> lazyConnection, IOptions<TLLSettings> options)
        {
            _lazyConnection = lazyConnection;
            Session = new ForSession(_lazyConnection, options);
        }
        public class ForSession
        {
            private Lazy<IConnectionMultiplexer> _lazyConnection;
            public readonly IOptions<TLLSettings> _options;
            public readonly ErrorContextCreator _errCreator;
            public ForSession(Lazy<IConnectionMultiplexer> lazyConnection, IOptions<TLLSettings> options)
            {
                _lazyConnection = lazyConnection;
                _options = options;
                _errCreator = new ErrorContextCreator(ServiceName.RedisService);
            }
            public async Task SafeSetSessionAsync(string refreshToken, int userId, string deviceId)
            {
                try
                {
                    await SetSessionAsync(refreshToken, userId, deviceId);
                }
                catch (RedisConnectionException ex)
                {
                    return;
                }
            }
            public async Task<bool?> SafeRevokeSessionAsync(int userId, string deviceId)
            {
                try
                {
                    return await RevokeSessionAsync(userId, deviceId);
                }
                catch (RedisConnectionException ex)
                {
                    return false;
                }
            }
            public async Task<SessionData?> SafeGetSessionAsync(int userId, string deviceId)
            {
                try
                {
                   return await GetSessionAsync(userId,deviceId);
                }
                catch (RedisConnectionException ex)
                {
                    return null;
                }
            }
            private async Task<bool> SetSessionAsync(string refreshToken,int userId, string deviceId)
            {
                var _db = _lazyConnection.Value.GetDatabase(0);
                string key = $"session:{userId}:{deviceId}";
                var entries = new HashEntry[]
                {
                new HashEntry("IsRevoked", false),
                new HashEntry("HashedToken", refreshToken)
                };
            await _db.HashSetAsync(key, entries);
            await _db.KeyExpireAsync(key, _options.Value.SessionExpires);

                return true;
            }
            private async Task<bool> RevokeSessionAsync(int userId, string deviceId)
            {
                var _db = _lazyConnection.Value.GetDatabase(0);
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
            private async Task<SessionData?> GetSessionAsync(int userId, string deviceId)
            {
                var _db = _lazyConnection.Value.GetDatabase(0);
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

