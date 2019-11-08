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
        public bool cleanSterileTiles = true;
        public bool cleanMetalTiles = false;
        public bool cleanAllCleanlinessTiles = false;
        public bool cleanSilverTiles = false;
        public int jobQueueMax = 15;
        public int jobScanRadius = 15;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref cleanKitchen, "cleanKitchen", true);
            Scribe_Values.Look(ref cleanLaboratory, "cleanLaboratory", true);
            Scribe_Values.Look(ref cleanPrisonBarracks, "cleanPrisonBarracks", false);
            Scribe_Values.Look(ref cleanHospital, "cleanHospital", true);
            Scribe_Values.Look(ref cleanCell, "cleanCell", false);
            Scribe_Values.Look(ref cleanSterileTiles, "cleanSterileTiles", true);
            Scribe_Values.Look(ref cleanMetalTiles, "cleanMetalTiles", false);
            Scribe_Values.Look(ref cleanAllCleanlinessTiles, "cleanAllCleanlinessTiles", false);
            Scribe_Values.Look(ref cleanSilverTiles, "cleanSilverTiles", false);
            Scribe_Values.Look(ref jobQueueMax, "jobQueueMax", 15);
            Scribe_Values.Look(ref jobScanRadius, "jobScanRadius", 5);
        }
    }
}
