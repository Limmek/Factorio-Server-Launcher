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
    class MapGeneratorSettingsExample
    {
        public static string FilePath;

        // Rewrite...
        public static void LoadExample(FormMain mainForm) 
        {
            JObject mapGenSettingsObject = JObject.Parse(File.ReadAllText(FilePath));
            JObject mapGenSettings = (JObject)mapGenSettingsObject;

            mainForm.console.AppendText(FilePath);
            mainForm.console.AppendText(mapGenSettings.ToString());
        }

    }
        

}
