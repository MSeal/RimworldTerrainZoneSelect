using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using UnityEngine;
using HarmonyLib;

namespace TerrainZoneSelect {
    public class Designator_GrowZoneSelect : Designator_ZoneAdd_Growing {
        public Designator_GrowZoneSelect()
        {
            this.zoneTypeToPlace = typeof(Zone_Growing);
            this.defaultLabel = "TerrainZoneSelect.TerrainGrowZone".Translate();
            this.defaultDesc = "TerrainZoneSelect.TerrainGrowZoneDesc".Translate();
            this.icon = ContentFinder<Texture2D>.Get("UI/Designators/TerrainZoneCreate_Growing", true);
        }

        public override int DraggableDimensions
        {
            get { return 0; }
        }

        public override void DesignateSingleCell(IntVec3 loc)
        {
            if (CanDesignateCell(loc).Accepted)
            {
                var sameTerrainCells = TerrainFindable.TilesSharingTerrainType(Map, loc, 25, (c => this.CanDesignateCell(c).Accepted));
                DesignateMultiCell(sameTerrainCells);
            }
        }
    }

    [HarmonyPatch(typeof(Zone_Growing), "GetZoneAddGizmos")]
    class ZoneGrowPatch {
        public static IEnumerable<Gizmo> GetZoneAddGizmos()
        {
            yield return DesignatorUtility.FindAllowedDesignator<Designator_GrowZoneSelect>();
            yield return DesignatorUtility.FindAllowedDesignator<Designator_ZoneAdd_Growing_Expand>();
            yield return DesignatorUtility.FindAllowedDesignator<Designator_ShrinkZoneSelect>();
        }

        static bool Prefix(ref IEnumerable<Gizmo> __result)
        {
            __result = GetZoneAddGizmos();
            return false;
        }
    }
}
