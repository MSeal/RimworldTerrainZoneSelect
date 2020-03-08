using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using UnityEngine;
using HarmonyLib;

namespace TerrainZoneSelect
{
    public class Designator_TerrainStockpileZoneSelect : Designator_ZoneAddStockpile_Resources {
        public Designator_TerrainStockpileZoneSelect()
        {
            this.zoneTypeToPlace = typeof(Zone_Stockpile);
            this.defaultLabel = "TerrainZoneSelect.TerrainStockpileZoneExpandResource".Translate();
            this.defaultDesc = "TerrainZoneSelect.TerrainStockpileZoneExpandResourceDesc".Translate();
            this.icon = ContentFinder<Texture2D>.Get("UI/Designators/TerrainZoneCreate_Stockpile", true);
            this.hotKey = null;
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

    public class Designator_TerrainStockpileZoneSelect_Resources : Designator_TerrainStockpileZoneSelect {
        public Designator_TerrainStockpileZoneSelect_Resources()
        {
            this.preset = StorageSettingsPreset.DefaultStockpile;
            this.defaultLabel = "TerrainZoneSelect.TerrainStockpileZoneExpandResource".Translate();
            this.defaultDesc = "TerrainZoneSelect.TerrainStockpileZoneExpandResourceDesc".Translate();
            this.icon = ContentFinder<Texture2D>.Get("UI/Designators/TerrainZoneCreate_Stockpile", true);
            this.hotKey = null;
            this.tutorTag = "ZoneAddStockpile_Resources";
        }

        protected override void FinalizeDesignationSucceeded()
        {
            base.FinalizeDesignationSucceeded();
            LessonAutoActivator.TeachOpportunity(ConceptDefOf.StorageTab, OpportunityType.GoodToKnow);
        }
    }

    public class Designator_TerrainStockpileZoneSelect_Dumping : Designator_TerrainStockpileZoneSelect {
        public Designator_TerrainStockpileZoneSelect_Dumping()
        {
            this.preset = StorageSettingsPreset.DumpingStockpile;
            this.defaultLabel = "TerrainZoneSelect.TerrainStockpileZoneExpandDumping".Translate();
            this.defaultDesc = "TerrainZoneSelect.TerrainStockpileZoneExpandDumpingDesc".Translate();
            this.icon = ContentFinder<Texture2D>.Get("UI/Designators/TerrainZoneCreate_Stockpile", true);
            this.hotKey = null;
        }

        protected override void FinalizeDesignationSucceeded()
        {
            base.FinalizeDesignationSucceeded();
            LessonAutoActivator.TeachOpportunity(ConceptDefOf.StorageTab, OpportunityType.GoodToKnow);
        }
    }

    public class Designator_ZoneAddStockpile_Dumping : Designator_ZoneAddStockpile {
        public Designator_ZoneAddStockpile_Dumping()
        {

        }

        protected override void FinalizeDesignationSucceeded()
        {
            base.FinalizeDesignationSucceeded();
            LessonAutoActivator.TeachOpportunity(ConceptDefOf.StorageTab, OpportunityType.GoodToKnow);
        }
    }

    [HarmonyPatch(typeof(Zone_Stockpile), "GetZoneAddGizmos")]
    class ZoneStockpilePatch {
        public static IEnumerable<Gizmo> GetZoneAddGizmos()
        {
            yield return DesignatorUtility.FindAllowedDesignator<Designator_TerrainStockpileZoneSelect>();
            yield return DesignatorUtility.FindAllowedDesignator<Designator_ZoneAddStockpile_Expand>();
            yield return DesignatorUtility.FindAllowedDesignator<Designator_ShrinkZoneSelect>();
        }
        
        static bool Prefix(ref IEnumerable<Gizmo> __result)
        {
            __result = GetZoneAddGizmos();
            return false;
        }
    }
}
