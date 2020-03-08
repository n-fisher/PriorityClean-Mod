using UnityEngine;
using Verse;

namespace PriorityClean
{
    class PriorityClean : Mod {
        public static PriorityCleanSettings settings;

        public PriorityClean(ModContentPack content) : base(content) {
            settings = GetSettings<PriorityCleanSettings>();
        }

        public override string SettingsCategory() => "PriorityClean";

        public override void DoSettingsWindowContents(Rect inRect) {
            string jobQueueMaxString = settings.jobQueueMax.ToString(),
                jobScanRadiusString = settings.jobScanRadius.ToString();

            Listing_Standard listing_Standard = new Listing_Standard();
            listing_Standard.Begin(inRect);

            listing_Standard.CheckboxLabeled("Clean Kitchens: ", ref settings.cleanKitchen);
            listing_Standard.CheckboxLabeled("Clean Hospitals: ", ref settings.cleanHospital);
            listing_Standard.CheckboxLabeled("Clean Laboratories: ", ref settings.cleanLaboratory);
            listing_Standard.CheckboxLabeled("Clean Prison Barracks: ", ref settings.cleanPrisonBarracks);
            listing_Standard.CheckboxLabeled("Clean Cells: ", ref settings.cleanCell);

            listing_Standard.CheckboxLabeled("Clean sterile tiles: ", ref settings.cleanSterileTiles);
            listing_Standard.CheckboxLabeled("Clean silver tiles: ", ref settings.cleanSilverTiles);
            listing_Standard.CheckboxLabeled("Clean metal tiles: ", ref settings.cleanMetalTiles);
            listing_Standard.CheckboxLabeled("Clean all other cleanliness tiles: ", ref settings.cleanAllCleanlinessTiles);

            listing_Standard.TextFieldNumericLabeled<int>("Max number of tiles to queue in a job (1-50): ", ref settings.jobQueueMax, ref jobQueueMaxString, 1, 50);
            listing_Standard.TextFieldNumericLabeled<int>("Radius of tiles to scan for filth (1-10): ", ref settings.jobScanRadius, ref jobScanRadiusString, 1, 10);
            listing_Standard.End();
            settings.Write();
        }
    }
}
