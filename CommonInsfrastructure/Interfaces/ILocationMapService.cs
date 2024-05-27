using CommonInsfrastructure.DTOs;
using CommonInsfrastructure.Models.ConfigurationModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonInsfrastructure.Interfaces
{
    public interface ILocationMapService
    {
        public string GetUnitForUsers(Int64 UserId);
        public List<LocationMapUnit> GetAllUnits();
        public List<LocationMapBuilding> GetAllBuildings();
        public TreeNode BuildBuildingTree(string parentId, List<string> unitIdsToKeep);
    }
}