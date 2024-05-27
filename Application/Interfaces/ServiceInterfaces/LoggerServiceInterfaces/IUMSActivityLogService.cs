using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.ServiceInterfaces.LoggerServiceInterfaces
{
    public interface IUMSActivityLogService
    {
        Task<bool> CreateLog(string type, string description, object extraData, int userId);

    }
}
