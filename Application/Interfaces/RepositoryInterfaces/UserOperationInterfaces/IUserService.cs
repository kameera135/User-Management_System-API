using Domain.DataTransferringObjects.ConfigDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AES.Application.Interfaces.RepositoryInterfaces.UserOperationInterfaces
{
    public interface IUserService
    {
        public void KeepUserInformation(long userID, string firstName, string lastName, string role, string email);

        public List<AsseteTreeDTO> CreateAsseteTreeForUser(long userID, List<AsseteTreeDTO> lstTree);
    }
}