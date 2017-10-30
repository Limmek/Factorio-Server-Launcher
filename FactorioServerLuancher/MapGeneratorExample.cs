using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace FactorioServerLauncher
{
    class MapGeneratorExample
    {
        public static string FilePath; // Setts in settings

        public static void LoadExample(FormMain mainForm)
        {
            JObject mapGenSettingsExampleObject = JObject.Parse(File.ReadAllText(FilePath));
            JObject mapGenSettings = (JObject)mapGenSettingsExampleObject;

            mainForm.mapGeneratorTerrainSegmentation.Text = mapGenSettings["terrain_segmentation"].ToString();
            mainForm.mapGeneratorWater.Text = mapGenSettings["water"].ToString();
            mainForm.mapGeneratorStartingArea.Text = mapGenSettings["starting_area"].ToString();

            //mainForm.mapGeneratorSeedComment.Text = mapGenSettings["_comment_seed"].ToString();
            mainForm.mapGeneratorSeed.Text = "null";

            mainForm.mapGeneratorWidth.Text = ((int)mapGenSettings["width"]).ToString();
            mainForm.mapGeneratorHeight.Text = ((int)mapGenSettings["height"]).ToString();

            mainForm.mapGeneratorPeacefulMode.Checked = ((Boolean)mapGenSettings["peaceful_mode"]);

            mainForm.mapGeneratorFrequencyCoal.Text = mapGenSettings["autoplace_controls"]["coal"]["frequency"].ToString();
            mainForm.mapGeneratorRichnessCoal.Text = mapGenSettings["autoplace_controls"]["coal"]["richness"].ToString();
            mainForm.mapGeneratorSizeCoal.Text = mapGenSettings["autoplace_controls"]["coal"]["size"].ToString();

            mainForm.mapGeneratorFrequencyCopperOre.Text = mapGenSettings["autoplace_controls"]["copper-ore"]["frequency"].ToString();
            mainForm.mapGeneratorRichnessCopperOre.Text = mapGenSettings["autoplace_controls"]["copper-ore"]["richness"].ToString();
            mainForm.mapGeneratorSizeCopperOre.Text = mapGenSettings["autoplace_controls"]["copper-ore"]["size"].ToString();

            mainForm.mapGeneratorFrequencyCrudeOil.Text = mapGenSettings["autoplace_controls"]["crude-oil"]["frequency"].ToString();
            mainForm.mapGeneratorRichnessCrudeOil.Text = mapGenSettings["autoplace_controls"]["crude-oil"]["richness"].ToString();
            mainForm.mapGeneratorSizeCrudeOil.Text = mapGenSettings["autoplace_controls"]["crude-oil"]["size"].ToString();

            mainForm.mapGeneratorFrequencyEnemyBase.Text = mapGenSettings["autoplace_controls"]["enemy-base"]["frequency"].ToString();
            mainForm.mapGeneratorRichnessEnemyBase.Text = mapGenSettings["autoplace_controls"]["enemy-base"]["richness"].ToString();
            mainForm.mapGeneratorSizeEnemyBase.Text = mapGenSettings["autoplace_controls"]["enemy-base"]["size"].ToString();

            mainForm.mapGeneratorFrequencyIronOre.Text = mapGenSettings["autoplace_controls"]["iron-ore"]["frequency"].ToString();
            mainForm.mapGeneratorRichnessIronOre.Text = mapGenSettings["autoplace_controls"]["iron-ore"]["richness"].ToString();
            mainForm.mapGeneratorSizeIronOre.Text = mapGenSettings["autoplace_controls"]["iron-ore"]["size"].ToString();

            mainForm.mapGeneratorFrequencyStone.Text = mapGenSettings["autoplace_controls"]["stone"]["frequency"].ToString();
            mainForm.mapGeneratorRichnessStone.Text = mapGenSettings["autoplace_controls"]["stone"]["richness"].ToString();
            mainForm.mapGeneratorSizeStone.Text = mapGenSettings["autoplace_controls"]["stone"]["size"].ToString();

            mainForm.mapGeneratorFrequencyUraniumOre.Text = mapGenSettings["autoplace_controls"]["uranium-ore"]["frequency"].ToString();
            mainForm.mapGeneratorRichnessUraniumOre.Text = mapGenSettings["autoplace_controls"]["uranium-ore"]["richness"].ToString();
            mainForm.mapGeneratorSizeUraniumOre.Text = mapGenSettings["autoplace_controls"]["uranium-ore"]["size"].ToString();

            mainForm.console.AppendText(FilePath);
            mainForm.console.AppendText(mapGenSettings.ToString());
        }
    }
}
