using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace PriorityClean
{
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

        private static bool IsInPriorityRoom(Filth f)
        {
            if (PriorityClean.settings.cleanAllOtherSterileTiles && IsOnSterileTile(f)) {
                return true;
            }

            switch (f.GetRoom()?.Role?.defName)
            {
                case "Kitchen":
                    return PriorityClean.settings.cleanKitchen;
                case "Laboratory":
                    return PriorityClean.settings.cleanLaboratory;
                case "PrisonBarracks":
                    return PriorityClean.settings.cleanPrisonBarracks;
                case "PrisonCell":
                    return PriorityClean.settings.cleanCell;
                case "Hospital":
                    return PriorityClean.settings.cleanHospital;
                default:
                    return false;
            }
        }

        private static bool IsOnSterileTile(Filth f)
        {
            return f.Map.terrainGrid.TerrainAt(f.InteractionCell).defName.Equals("SterileTile");
        }

        private static bool IsPriorityFilth(Filth f)
        {
            return IsInPriorityRoom(f) &&
                !IsOnBuggyTile(f);
        }

        private static bool IsOnBuggyTile(Filth f)
        {
            return f.Map.thingGrid.ThingsListAt(f.InteractionCell).Any(t => t is Blueprint || t is Frame);
        }
        
        public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            return pawn.Faction == Faction.OfPlayer &&
                t is Filth filth && 
                IsPriorityFilth(filth) &&
                filth.Map.areaManager.Home[filth.Position] && 
                pawn.CanReserve(filth, 1, -1, null, forced);
         }

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            Job job = new Job(JobDefOf.Clean);
            Map map = t.Map;
            Room room = t.GetRoom(RegionType.Set_Passable);

            if (t is Filth f && IsPriorityFilth(f))
            {
                job.AddQueuedTarget(TargetIndex.A, t);
                foreach (IntVec3 intVec in GenRadial.RadialPatternInRadius(PriorityClean.settings.jobScanRadius)) {
                    IntVec3 pos = intVec + t.Position;
                    if (pos.InBounds(map) && pos.GetRoom(map, RegionType.Set_Passable) == room) {
                        pos.GetThingList(map).ForEach(thing => {
                            if (thing != t && HasJobOnThing(pawn, thing, forced)) {
                                job.AddQueuedTarget(TargetIndex.A, thing);
                            }
                        });

                        if (job.GetTargetQueue(TargetIndex.A).Count >= PriorityClean.settings.jobQueueMax) { 
                            break;
                        }
                    }
                }
            }
            if (job.targetQueueA?.Count >= 5) {
                job.targetQueueA.SortBy(targ => targ.Cell.DistanceToSquared(pawn.Position));
            }
            return job;
        }
    }
}

