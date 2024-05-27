using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AES.Application.Interfaces.RepositoryInterfaces.ActivityLogInterfaces
{
    public interface IActivityLogRepository
    {

        public bool InsertActivityLog(long userID, string role, string action, string extensionID, string soID, DateTime createdAt, DateTime updatedAt);


    }
}
