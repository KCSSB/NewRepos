using API.DTO.Responses;
using DataBaseInfo;
using API.DTO.Responses.Pages.HomePage;
using API.DTO.Responses.Pages.SettingsPage;
using API.DTO.Responses.Pages.HallPage;
namespace API.Services.Application.Interfaces
{

    public interface IGetPagesService
    {
        public Task<HomePage> CreateHomePageDTOAsync(int userId);
        public Task<SettingsPage> CreateSettingsPageDTOAsync(int userId);
        public Task<HallPage?> CreateHallPageDTOAsync(int userId, int projectId);
    }
}
