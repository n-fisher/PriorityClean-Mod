using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace PriorityClean
{
    public static class PriorityCleanDefOf
    {
        public static TerrainDef SterileTile = TerrainDef.Named("SterileTile");
    }

    public class WorkGiver_PriorityClean : WorkGiver_Scanner
    {
        public override PathEndMode PathEndMode
        {
            get => PathEndMode.Touch;
        }

        public override ThingRequest PotentialWorkThingRequest
        {
            get => ThingRequest.ForGroup(ThingRequestGroup.Filth);
        }

        public override int LocalRegionsToScanFirst
        {
            get => 4;
        }

        public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
        {
            List<Thing> list = pawn.Map.listerFilthInHomeArea.FilthInHomeArea;
            list.RemoveAll(filth => !IsPriorityFilth((Filth)filth));
            return list;
        }

        private List<RoomRoleDef> allowedRooms = new List<RoomRoleDef>() { RoomRoleDefOf.Hospital, RoomRoleDefOf.Laboratory, RoomRoleDefOf.PrisonBarracks, RoomRoleDefOf.PrisonCell };

        private bool IsPriorityRoom(RoomRoleDef room)
        {
            return room.defName.Equals("Kitchen") || allowedRooms.Contains(room);
        }

        private bool IsPriorityFilth(Filth f)
        {
            return IsPriorityRoom(f.GetRoom().Role) && f.Map.terrainGrid.TerrainAt(f.InteractionCell).defName.Equals("SterileTile") && !f.Map.thingGrid.ThingsListAt(f.InteractionCell).Any(t => !(t is Blueprint));
        }
        
        public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            return pawn.Faction == Faction.OfPlayer &&
                t != null &&
                t is Filth && 
                IsPriorityFilth((Filth) t) && 
                t.Map.areaManager.Home[t.Position] && 
                pawn.CanReserve(t, 1, -1, null, forced);
         }

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            Job job = new Job(JobDefOf.Clean);
            const int queueMax = 10;
            const int searchDepthMax = 100;
            Map map = t.Map;
            Room room = t.GetRoom(RegionType.Set_Passable);

            if (IsPriorityRoom(room.Role))
            {
                job.AddQueuedTarget(TargetIndex.A, t);
                for (int i = 0; i < searchDepthMax; i++)
                {
                    IntVec3 intVec = t.Position + GenRadial.RadialPattern[i];
                    if (intVec.InBounds(map) &&
                        intVec.GetRoom(map, RegionType.Set_Passable) == room)
                    {
                        List<Thing> thingList = intVec.GetThingList(map);
                        for (int j = 0; j < thingList.Count; j++)
                        {
                            Thing thing = thingList[j];
                            if (HasJobOnThing(pawn, thing, forced) && thing != t)
                            {
                                job.AddQueuedTarget(TargetIndex.A, thing);
                            }
                        }
                        if (job.GetTargetQueue(TargetIndex.A).Count >= queueMax)
                            break;
                    }
                    else
                    {
                        // If breaking out of container bump index along instead of continuing expensive checks for what could be a stretch of wall
                        i = (int)(i * 1.3);
                    }
                }
            }
            if (job.targetQueueA != null && job.targetQueueA.Count >= 5)
            {
                job.targetQueueA.SortBy((LocalTargetInfo targ) => targ.Cell.DistanceToSquared(pawn.Position));
            }
            return job;
        }
    }
}

