using Verse;

namespace PriorityClean
{
    class PriorityCleanSettings : ModSettings
    {
        public bool cleanHospital = true;
        public bool cleanLaboratory = true;
        public bool cleanPrisonBarracks = false;
        public bool cleanCell = false;
        public bool cleanKitchen = true;
        public bool cleanAllOtherSterileTiles = true;
        public int jobQueueMax = 15;
        public int jobScanRadius = 15;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref cleanKitchen, "cleanKitchen", true);
            Scribe_Values.Look(ref cleanLaboratory, "cleanLaboratory", true);
            Scribe_Values.Look(ref cleanPrisonBarracks, "cleanPrisonBarracks", true);
            Scribe_Values.Look(ref cleanHospital, "cleanHospital", true);
            Scribe_Values.Look(ref cleanCell, "cleanCell", true);
            Scribe_Values.Look(ref cleanAllOtherSterileTiles, "cleanAllOtherSterileTiles", true);
            Scribe_Values.Look(ref jobQueueMax, "jobQueueMax", 15);
            Scribe_Values.Look(ref jobScanRadius, "jobScanRadius", 5);
        }
    }
}
