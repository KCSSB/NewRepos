namespace API.Services.Helpers.Interfaces.Redis
{
    interface IRedisService
    {
        IRedisSession Session { get; }
    }
}