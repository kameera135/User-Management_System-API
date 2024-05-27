using AES.Application.Interfaces.RepositoryInterfaces.CommonUtilityInterfaces;
using AES.Application.Interfaces.RepositoryInterfaces.UserOperationInterfaces;
using Domain.DataTransferringObjects.CommonUtilities;
using Domain.DataTransferringObjects.ConfigDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AES.Application.ApplicationClasses.UserClasses
{
    public class UserService : IUserService
    {
        private readonly IUserOperationsRepository m_userOperationsRepository;

        private readonly ICommonUtilitiesRepository m_commonRepo;

        private List<string> lstUserUnits { get; set; }

        public UserService(IUserOperationsRepository userOperationsRepository, ICommonUtilitiesRepository commnRepo)
        {
            m_userOperationsRepository = userOperationsRepository;

            m_commonRepo = commnRepo;
        }

        public void KeepUserInformation(long userID, string firstName, string lastName, string role, string email)
        {
            m_userOperationsRepository.KeepUserInformation(userID, firstName, lastName, role, email);
        }

        public List<AsseteTreeDTO> CreateAsseteTreeForUser(long userID, List<AsseteTreeDTO> lstTree)
        {
            LoadAllUnitsForUser(userID);

            var result = traverseAll(lstTree);

            return (List<AsseteTreeDTO>)RecreateTree(result);
        }

        private void LoadAllUnitsForUser(long userID)
        {
            List<ExtensionSavingHelperDTO> lstTemp = m_commonRepo.LoadUnitsByUser(userID);

            lstUserUnits = new List<string>();

            if (lstTemp != null && lstTemp.Count > 0)
            {
                foreach (var itm in lstTemp)
                {
                    lstUserUnits.Add(itm.unitName);
                }
            }
        }

        private List<AsseteTreeDTO> traverseAll(List<AsseteTreeDTO>? lstTree)
        {
            if (lstTree != null)
            {
                foreach (AsseteTreeDTO node in lstTree)
                {
                    if (node.isUnit)
                    {
                        if (lstUserUnits != null && lstUserUnits.Count > 0)
                        {
                            if (lstUserUnits.Contains(node.name))
                            {
                                node.haveRightToView = true;

                                node.selected = false;
                            }
                            else
                            {
                                node.haveRightToView = false;
                            }
                        }
                        else
                        {
                            node.haveRightToView = false;
                        }
                    }
                    else
                    {
                        traverseAll(node.children);
                    }
                }
            }

            return lstTree;
        }

        //This method recreate a tree bases on the user previladges
        private object RecreateTree(List<AsseteTreeDTO>? lstTree)
        {
            List<AsseteTreeDTO> objTemp = new List<AsseteTreeDTO>();

            bool hasUnits = false;

            if (lstTree != null)
            {
                foreach (AsseteTreeDTO node in lstTree)
                {
                    if (node.isUnit)
                    {
                        hasUnits = true;

                        if (node.haveRightToView)
                        {
                            objTemp.Add(node);
                        }
                    }
                    else
                    {
                        node.children = (List<AsseteTreeDTO>)RecreateTree(node.children);
                    }
                }
            }

            if (objTemp != null && objTemp.Count >= 0 && hasUnits)
            {
                return objTemp;
            }

            return lstTree;
        }
    }
}