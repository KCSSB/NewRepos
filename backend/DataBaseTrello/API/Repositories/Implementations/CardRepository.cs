using API.Repositories.Interfaces;
using DataBaseInfo;
using DataBaseInfo.models;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Implementations
{
    public class CardRepository: ICardRepository
    {
        private readonly AppDbContext _context;
        public CardRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<Card?> GetCardAsync(int cardId)
        {
            return await _context.Cards.FirstOrDefaultAsync(c => c.Id == cardId);
        }
        public async Task DeleteCardAsync(Card card)
        {
            _context.Cards.Remove(card);
        }
        public async Task AddCardAsync(Card card)
        {
            await _context.Cards.AddAsync(card);
   
        }
    }
}
