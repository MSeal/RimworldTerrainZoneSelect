using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using UnityEngine;

namespace TerrainZoneSelect
{
    public class Designator_ShrinkZoneSelect : Designator_ZoneDelete {
        protected HashSet<Zone> justDesignated = new HashSet<Zone>();

        public Designator_ShrinkZoneSelect()
        {
            this.defaultLabel = "TerrainZoneSelect.TerrainShrinkZone".Translate();
            this.defaultDesc = "TerrainZoneSelect.TerrainShrinkZoneDesc".Translate();
            this.icon = ContentFinder<Texture2D>.Get("UI/Designators/TerrainZoneDelete", true);
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
                Zone zone = base.Map.zoneManager.ZoneAt(loc);
                // No limit needed for removal (10000)
                var sameTerrainCells = TerrainFindable.TilesSharingTerrainType(Map, loc, 10000);
                foreach (var cell in sameTerrainCells)
                {
                    if (CanDesignateCell(cell).Accepted && base.Map.zoneManager.ZoneAt(cell) == zone)
                    {
                        zone.RemoveCell(cell);
                        this.justDesignated.Add(zone);
                    }
                }
            }
        }

        protected override void FinalizeDesignationSucceeded()
        {
            base.FinalizeDesignationSucceeded();
            foreach (Zone zone in justDesignated)
            {
                zone.CheckContiguous();
            }
            this.justDesignated.Clear();
        }
    }
}
