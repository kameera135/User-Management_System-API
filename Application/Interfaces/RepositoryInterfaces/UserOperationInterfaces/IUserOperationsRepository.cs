using Domain.DataTransferringObjects.UserDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AES.Application.Interfaces.RepositoryInterfaces.UserOperationInterfaces
{
    public interface IUserOperationsRepository
    {
        public void KeepUserInformation(long userID, string firstName, string lastName, string role, string email);

        public UserNameDTO? GetUserInfo(long userId);
    }
}