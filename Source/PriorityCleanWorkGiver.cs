using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace PriorityClean
{
    public class WorkGiver_PriorityClean : WorkGiver_Scanner {
        public override PathEndMode PathEndMode {
            get => PathEndMode.Touch;
        }

        public override ThingRequest PotentialWorkThingRequest {
            get => ThingRequest.ForGroup(ThingRequestGroup.Filth);
        }

        public override int MaxRegionsToScanBeforeGlobalSearch {
            get => 4;
        }

        public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn) {
            return pawn.Map.listerFilthInHomeArea.FilthInHomeArea.FindAll(filth => IsPriorityFilth((Filth)filth));
        }

        private static bool IsPriorityFilth(Filth f) {
            bool isPriority = false;
            TerrainDef terrain = f?.Map?.terrainGrid?.TerrainAt(f.InteractionCell);
            if (terrain == null) return false;

            // check for priority tile types
            switch (terrain?.defName) {
                case "SterileTile":
                    isPriority = PriorityClean.settings.cleanSterileTiles; break;
                case "SilverTile":
                    isPriority = PriorityClean.settings.cleanSilverTiles; break;
                case "MetalTile":
                    isPriority = PriorityClean.settings.cleanMetalTiles; break;
                default:
                    isPriority = PriorityClean.settings.cleanAllCleanlinessTiles &&
                        terrain.statBases != null &&
                        terrain.statBases.Exists(def =>
                            def?.stat?.label == "cleanliness" &&
                            def.value > 0);
                    break;
            }

            // otherwise check for priority rooms
            if (!isPriority) {
                switch (f.GetRoom()?.Role?.defName) {
                    case "Kitchen":
                        isPriority = PriorityClean.settings.cleanKitchen; break;
                    case "Laboratory":
                        isPriority =  PriorityClean.settings.cleanLaboratory; break;
                    case "PrisonBarracks":
                        isPriority =  PriorityClean.settings.cleanPrisonBarracks; break;
                    case "PrisonCell":
                        isPriority =  PriorityClean.settings.cleanCell; break;
                    case "Hospital":
                        isPriority =  PriorityClean.settings.cleanHospital; break;
                }
            }

            // Finish with buggy tile check if it's a priority tile
            return isPriority && !IsOnBuggyTile(f);
        }

        private static bool IsOnBuggyTile(Filth f) {
            return f.Map.thingGrid.ThingsListAt(f.InteractionCell).Any(t => t is Blueprint || t is Frame);
        }
        
        public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false) {
            return pawn.IsColonistPlayerControlled &&
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

            if (t is Filth f && IsPriorityFilth(f)) {
                job.AddQueuedTarget(TargetIndex.A, t);
                foreach (IntVec3 intVec in GenRadial.RadialPatternInRadius(PriorityClean.settings.jobScanRadius)) {
                    IntVec3 pos = intVec + t.Position;
                    if (pos.InBounds(map) && pos.GetRoom(map) == room) {
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

