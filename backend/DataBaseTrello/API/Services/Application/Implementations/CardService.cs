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
            Card card = new Card();
            
            await _unitOfWork.CardRepository.AddCardAsync(card);

            board.Cards.Add(card);

            await _unitOfWork.SaveChangesAsync(ServiceName,"Ошибка при создании карточки");

            return card;
        }
    }
}
