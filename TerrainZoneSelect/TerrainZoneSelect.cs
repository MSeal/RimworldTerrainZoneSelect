using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using HarmonyLib;
using System.Reflection;

namespace TerrainZoneSelect
{
    static class Extensions {
        public static T Pop<T>(this HashSet<T> items) {
            T item = items.FirstOrDefault();
            if (item != null) {
                items.Remove(item);
            }
            return item;
        }
    }

    [StaticConstructorOnStartup]
    public static class TerrainZoneSelectLoader {
        static TerrainZoneSelectLoader()
        {
            var harmony = new Harmony("net.mseal.rimworld.mod.terrainzoneselect");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }

    static class TerrainFindable {
        public static IEnumerable<IntVec3> DirectlyConnectedCells(IntVec3 center)
        {
            yield return new IntVec3(center.x + 1, 0, center.z);
            yield return new IntVec3(center.x, 0, center.z + 1);
            yield return new IntVec3(center.x - 1, 0, center.z);
            yield return new IntVec3(center.x, 0, center.z - 1);
        }

        public static bool HasDoor(Map map, IntVec3 loc, bool debug = false) {
            Thing t = map.thingGrid.ThingAt(loc, ThingCategory.Building);
            return t != null && t.GetType().IsAssignableFrom(typeof(Building_Door));
        }

        public static void AddAdjacentFringeCellsSameType(Map map, IntVec3 center, HashSet<IntVec3> fringe, HashSet<IntVec3> explored, CellRect bounds, Func<IntVec3, bool> allowedCell = null, bool skipDoors = true)
        {
            foreach (IntVec3 cell in DirectlyConnectedCells(center))
            {
                if (!explored.Contains(cell) &&
                    bounds.Contains(cell) &&
                    cell.InBounds(map) &&
                    map.terrainGrid.TerrainAt(cell) == map.terrainGrid.TerrainAt(center) &&
                    (!skipDoors || !HasDoor(map, cell)) &&
                    (allowedCell == null || allowedCell(cell)))
                {
                    fringe.Add(cell);
                }
            }
        }

        public static List<IntVec3> TilesSharingTerrainType(Map map, IntVec3 loc, int radius, Func<IntVec3, bool> allowedCell = null, bool? skipDoors = null)
        {
            HashSet<IntVec3> explored = new HashSet<IntVec3>();
            HashSet<IntVec3> fringe = new HashSet<IntVec3>();
            CellRect bounds = CellRect.CenteredOn(loc, radius);
            if (skipDoors == null)
            {
                skipDoors = !HasDoor(map, loc, true);
            }

            fringe.Add(loc);
            while (fringe.Any())
            {
                IntVec3 nextCell = fringe.Pop();
                explored.Add(nextCell);
                AddAdjacentFringeCellsSameType(map, nextCell, fringe, explored, bounds, allowedCell, (bool)skipDoors);
            }

            // Force the clicked cell to be first in the list (it's usually used to select zones)
            explored.Remove(loc);
            List<IntVec3> sameTerrainCells = explored.ToList();
            sameTerrainCells.Insert(0, loc);
            return sameTerrainCells;
        }
    }
}
