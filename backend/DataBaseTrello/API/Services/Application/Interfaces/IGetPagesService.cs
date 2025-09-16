using API.DTO.Responses.Pages;
using API.DTO.Responses;
using DataBaseInfo;
namespace API.Services.Application.Interfaces
{

    public interface IGetPagesService
    {
        public Task<HomePage> CreateHomePageDTOAsync(int userId);
        public Task<SettingsPage> CreateSettingsPageDTOAsync(int userId);
       
    }
}
