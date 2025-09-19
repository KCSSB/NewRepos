using API.DTO.Responses;
using DataBaseInfo;
using API.DTO.Responses.Pages.HomePage;
using API.DTO.Responses.Pages.SettingsPage;
namespace API.Services.Application.Interfaces
{

    public interface IGetPagesService
    {
        public Task<HomePage> CreateHomePageDTOAsync(int userId);
        public Task<SettingsPage> CreateSettingsPageDTOAsync(int userId);
       
    }
}
