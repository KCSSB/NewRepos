using DataBaseInfo.models;

namespace API.Repositories.Interfaces
{
    public interface ICardRepository
    {
        public Task<Card?> GetCardAsync(int cardId);
        public Task DeleteCardAsync(Card card);
        public Task AddCardAsync(Card card);
    }
}
