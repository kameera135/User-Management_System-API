using CommonInsfrastructure.DTOs;
using CommonInsfrastructure.Interfaces;
using CommonInsfrastructure.Models.ConfigurationModels;
using LoggerLibrary.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonInsfrastructure.Repositories
{
    public class LocationMapRepository : ILocationMapService
    {
        private readonly ILogService m_logService;

        private readonly ConfigContext m_configDBContext;

        public LocationMapRepository(ILogService logService, ConfigContext configDBContext)
        {
            m_logService = logService;
            m_configDBContext = configDBContext;
        }

        public string GetUnitForUsers(Int64 UserId)
        {
            //List<LocationMapUnitUserMapping> unitList = new();
            var unitList = m_configDBContext.LocationMapUnitUserMappings.Where(q => q.DeletedAt == null && q.UserId == UserId).ToList();

            if (unitList.Count == 0 || unitList == null)
            {
                return "[]"; // Return an empty JSON array as a string
            }
            var jsonUnitList = System.Text.Json.JsonSerializer.Serialize(unitList);

            return jsonUnitList;
        }

        public List<LocationMapUnit> GetAllUnits()
        {
            var unitListFullInfo = m_configDBContext.LocationMapUnits.Where(q => q.DeletedAt == null).ToList();

            if(unitListFullInfo.Count == 0)
            {
                return null;
            }
            return unitListFullInfo;
        }

        public List<LocationMapBuilding> GetAllBuildings()
        {
            var buildings = m_configDBContext.LocationMapBuildings.Where(q => q.DeletedAt == null).ToList();

            if (buildings.Count == 0)
            {
                return null;
            }
            return buildings;
        }

        public TreeNode BuildBuildingTree(string parentId, List<string> unitIdsToKeep)
        {
            var buildingInfo = m_configDBContext.LocationMapBuildings
                                .Where(q => q.DeletedAt == null && q.BuildingId == parentId)
                                .Select(q => new LocationMapBuilding
                                {
                                    // Map the properties from the LocationMapBuildings entity to your data model
                                    Name = q.Name,
                                    BuildingId = q.BuildingId,
                                })
                                .FirstOrDefault();

            if (buildingInfo == null)
            {
                m_logService.Log("Building information not found for parentId: " + parentId);
                return null;
            }


            var buildingNode = new TreeNode
            {
                name = buildingInfo.Name,
                id = parentId,
                icon = "Building",
                isUnit = false,
                children = new List<TreeNode>()
            };

            var levels = m_configDBContext.LocationMapLevels
                .Where(level => level.BuildingId == parentId && level.ParentId == null && level.DeletedAt == null)
                .Select(q => new LocationMapLevel
                {
                    Name = q.Name,
                    LevelId = q.LevelId,
                    Icon = q.Icon,
                })
                .ToList();

            // Check if any child levels or specified units are associated with this building or its hierarchy
            var hasAssociatedUnits = levels.Any(level => IsUnitOrChildUnit(level.LevelId, unitIdsToKeep));

            foreach (var level in levels)
            {
                var levelNode = new TreeNode
                {
                    name = level.Name,
                    id = level.LevelId,
                    icon = level.Icon,
                    isUnit = false,
                    children = new List<TreeNode>()
                };

                // Recursive function to build child levels and units, but only for the selected unit IDs
                BuildChildLevelsAndUnits(levelNode, level.LevelId, unitIdsToKeep);

                if (levelNode.children.Any() || IsUnitOrChildUnit(level.LevelId, unitIdsToKeep))
                {
                    buildingNode.children.Add(levelNode);
                }
            }

            if (buildingNode.children.Any() || hasAssociatedUnits)
            {
                return buildingNode;
            }
            return null; // Skip buildings not associated with specified units
        }


        public bool IsUnitOrChildUnit(string parentId, List<string> unitIdsToKeep)
        {
            if (unitIdsToKeep.Contains(parentId))
            {
                return true;
            }

            var levels = m_configDBContext.LocationMapLevels
                .Where(level => level.ParentId == parentId && level.DeletedAt == null)
                .ToList();

            foreach (var level in levels)
            {
                if (IsUnitOrChildUnit(level.LevelId, unitIdsToKeep))
                {
                    return true;
                }
            }

            return false;
        }

        public void BuildChildLevelsAndUnits(TreeNode parentNode, string parentId, List<string> unitIdsToKeep)
        {
            // Fetch units for this level
            var units = m_configDBContext.LocationMapUnits
                .Where(unit => unit.ParentLevelId == parentId && unit.DeletedAt == null)
                .ToList();

            foreach (var unit in units)
            {
                var unitNode = new TreeNode
                {
                    name = unit.Name,
                    id = unit.UnitId,
                    icon = "Unit",
                    isUnit = true
                };

                if (unitIdsToKeep.Contains(unit.UnitId))
                {
                    parentNode.children.Add(unitNode);
                }
            }

            var levels = m_configDBContext.LocationMapLevels
                .Where(level => level.ParentId == parentId && level.DeletedAt == null)
                .ToList();

            foreach (var level in levels)
            {
                var levelNode = new TreeNode
                {
                    name = level.Name,
                    id = level.LevelId,
                    icon = "Level",
                    isUnit = false,
                    children = new List<TreeNode>()
                };

                // Recursive function to build child levels and units, but only for the selected unit IDs
                BuildChildLevelsAndUnits(levelNode, level.LevelId, unitIdsToKeep);

                if (levelNode.children.Any() || unitIdsToKeep.Contains(level.LevelId))
                {
                    parentNode.children.Add(levelNode);
                }
            }
        }








    }
}