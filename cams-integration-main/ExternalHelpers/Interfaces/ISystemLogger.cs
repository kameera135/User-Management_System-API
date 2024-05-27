using System.Threading.Tasks;

namespace SITCAMSClientIntegration.ExternalHelpers.Interfaces
{
    public interface ISystemLogger
    {
        /// <summary>
        /// This will create new log record in CAMS for the specific platform
        /// </summary>
        /// <param name="type">Any string</param>
        /// <param name="description"> Brief Description</param>
        /// <param name="extraData">Any class can be passed into this. Use for additional log information</param>
        /// <param name="userId">var tokenUserId = int.Parse(HttpContext.User.FindFirst("UserId").Value); (HttpContext will be inherited from ControllerBase)</param>
        /// <returns> true if log created successfully</returns>
        Task<bool> CreateLog(string type, string description, object extraData, int userId);
    }
}