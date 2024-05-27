using SITCAMSClientIntegration.DTOs;
using System.Threading.Tasks;

namespace SITCAMSClientIntegration.ExternalHelpers.Interfaces
{
    public interface IUserInfo
    {
        Task<EmployeeDTO> GetUserDetails(int userId, string userToken);

        Task<PaginationDTO<EmployeeWithRoleDTO>> GetAllUsersByPlatform(int page = 0, int limit = 0);
    }
}