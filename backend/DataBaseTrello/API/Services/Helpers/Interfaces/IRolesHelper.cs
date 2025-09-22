namespace API.Services.Helpers.Interfaces
{
    public interface IRolesHelper
    {
        public Task IsProjectOwner(int userId, int projectId);
    }
}
