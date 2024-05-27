using CommonInsfrastructure.Models.EmailModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonInsfrastructure.Interfaces
{
    public interface IEmailService
    {
        public void SendEmail(TblEmailList email);
    }
}