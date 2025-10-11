using System.Security.Authentication.ExtendedProtection;
using API.Exceptions.Context;
using API.Exceptions.ContextCreator;
using API.Repositories.Uof;
using API.Services.Application.Interfaces;
using DataBaseInfo.models;

namespace API.Services.Application.Implementations
{
    public class CardService
    {
        private readonly string ServiceName = nameof(CardService);
        private readonly IUnitOfWork _unitOfWork;
        private readonly IErrorContextCreatorFactory _errCreatorFactory;
        private ErrorContextCreator? _errorContextCreator;
        private ErrorContextCreator _errCreator => _errorContextCreator ??= _errCreatorFactory.Create(nameof(IGetPagesService));
        public CardService(IUnitOfWork unitOfWork, IErrorContextCreatorFactory errCreatorFactory)
        {
            _unitOfWork = unitOfWork;
            _errCreatorFactory = errCreatorFactory;
        }
        public async Task<Card?> CreateCardAsync(int boardId)
        {
            Board? board = await _unitOfWork.BoardRepository.GetAsync(boardId);
            if (board == null)
                throw new AppException(_errCreator.NotFound("Доска не найдена"));
            Card card = new Card()
            {
                BoardId = boardId,
            };
            
            await _unitOfWork.CardRepository.AddCardAsync(card);

            board.Cards.Add(card);

            await _unitOfWork.SaveChangesAsync(ServiceName,"Ошибка при создании карточки");

            return card;
        }
        public async Task DeleteCardsAsync(List<int> CardIds) 
        {
            int count = 0;
            foreach(var cardId in CardIds)
            {
  
                var card = await _unitOfWork.CardRepository.GetCardAsync(cardId);
                if (card != null)
                {
                    await _unitOfWork.CardRepository.DeleteCardAsync(card);
                    count++;
                }
            }
            if (count==0)
                throw new AppException(_errCreator.NotFound("Карточка не найдена"));
            await _unitOfWork.SaveChangesAsync(ServiceName, "Ошибки при удалении карточек");
        }
    }
}
