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
    class MapGenerator
    {
        public static string GeneratorArguments;
        public static int Pid = 0;
        public static Process GenerateMap = new Process();

        public static void SaveSettings(FormMain mainForm)
        {
            JObject mapGenSettingsObject = new JObject();
            JObject mapGenSettings = (JObject)mapGenSettingsObject;

            mapGenSettings["peaceful_mode"] = Convert.ToBoolean(mainForm.mapGeneratorPeacefulMode.Checked);

            mapGenSettings["width"] = Convert.ToInt32(mainForm.mapGeneratorWidth.Text);
            mapGenSettings["height"] = Convert.ToInt32(mainForm.mapGeneratorHeight.Text);

            if ((mainForm.mapGeneratorSeed.Text == "null") || (mainForm.mapGeneratorSeed.Text == ""))
                mapGenSettings["seed"] = null;
            else
                mapGenSettings["seed"] = Convert.ToInt32(mainForm.mapGeneratorSeed.Text);

            mapGenSettings["terrain_segmentation"] = mainForm.mapGeneratorTerrainSegmentation.Text;
            mapGenSettings["water"] = mainForm.mapGeneratorWater.Text;
            mapGenSettings["starting_area"] = mainForm.mapGeneratorStartingArea.Text;

            JObject coal = new JObject();
            coal.Add("frequency", mainForm.mapGeneratorFrequencyCoal.Text);
            coal.Add("richness", mainForm.mapGeneratorRichnessCoal.Text);
            coal.Add("size", mainForm.mapGeneratorSizeCoal.Text);

            JObject copperOre = new JObject();
            copperOre.Add("frequency", mainForm.mapGeneratorFrequencyCopperOre.Text);
            copperOre.Add("richness", mainForm.mapGeneratorRichnessCopperOre.Text);
            copperOre.Add("size", mainForm.mapGeneratorSizeCopperOre.Text);

            JObject crudeOil = new JObject();
            crudeOil.Add("frequency", mainForm.mapGeneratorFrequencyCrudeOil.Text);
            crudeOil.Add("richness", mainForm.mapGeneratorRichnessCrudeOil.Text);
            crudeOil.Add("size", mainForm.mapGeneratorSizeCrudeOil.Text);

            JObject enemyBase = new JObject();
            enemyBase.Add("frequency", mainForm.mapGeneratorFrequencyEnemyBase.Text);
            enemyBase.Add("richness", mainForm.mapGeneratorRichnessEnemyBase.Text);
            enemyBase.Add("size", mainForm.mapGeneratorSizeEnemyBase.Text);

            JObject ironOre = new JObject();
            ironOre.Add("frequency", mainForm.mapGeneratorFrequencyIronOre.Text);
            ironOre.Add("richness", mainForm.mapGeneratorRichnessIronOre.Text);
            ironOre.Add("size", mainForm.mapGeneratorSizeIronOre.Text);

            JObject stone = new JObject();
            stone.Add("frequency", mainForm.mapGeneratorFrequencyStone.Text);
            stone.Add("richness", mainForm.mapGeneratorRichnessStone.Text);
            stone.Add("size", mainForm.mapGeneratorSizeStone.Text);

            JObject uraniumOre = new JObject();
            uraniumOre.Add("frequency", mainForm.mapGeneratorFrequencyUraniumOre.Text);
            uraniumOre.Add("richness", mainForm.mapGeneratorRichnessUraniumOre.Text);
            uraniumOre.Add("size", mainForm.mapGeneratorSizeUraniumOre.Text);

            JObject autoplaceControls = new JObject();
            autoplaceControls.Add("coal", coal);
            autoplaceControls.Add("copper-ore", copperOre);
            autoplaceControls.Add("crude-oil", crudeOil);
            autoplaceControls.Add("enemy-base", enemyBase);
            autoplaceControls.Add("iron-ore", ironOre);
            autoplaceControls.Add("stone", stone);
            autoplaceControls.Add("uranium-ore", uraniumOre);

            mapGenSettings["autoplace_controls"] = autoplaceControls;

            File.WriteAllText(Settings.factorioLauncherMapGenSettingsFile, mapGenSettings.ToString());
        }

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public static void Generate(string name)
        {
            GeneratorArguments = " --create " + Settings.factorioSavesPath + name + ".zip" +
                                    " --map-gen-settings " + Settings.factorioLauncherMapGenSettingsFile +
                                    " --map-settings " + Settings.factorioLauncherMapSettingsFile;

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.WindowStyle = ProcessWindowStyle.Normal;
            startInfo.Verb = "runas";
            startInfo.FileName = Settings.factorioEXE;
            startInfo.Arguments = GeneratorArguments;
            startInfo.WorkingDirectory = Path.GetDirectoryName(Settings.factorioEXE);
            startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = true;

            GenerateMap = Process.Start(startInfo);
            Pid = GenerateMap.Id;
            Console.WriteLine(GeneratorArguments);

        }

    }
}
