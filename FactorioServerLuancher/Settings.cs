using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Permissions;
using Newtonsoft.Json.Linq;
using System.Net;

namespace FactorioServerLauncher
{
    class Settings
    {
        public static string appDataPath;
        public static string programPath;
        public static string programX86Path;

        public static string steamPath;                                     // Steam

        public static string factorioInstallPath;                           // Factorio installation folder
        public static string factorioPath;                                  // Factorio resources 
        public static string factorioModsPath;                              //Factorio Mod folderfolder
        public static string factorioSavesPath;                             // Factorio save files
        public static string factorioConfig;                                // Factorio config file
        public static string factorioEXEPath;                               // Path to Factorio.exe
        public static string factorioEXE;                                   // Factorio.exe
        
        public static string factorioLauncherPath;                          // Launcher resources folder
        public static string factorioLauncherConfig;                        // Factorio launcher config file
        public static string factorioLauncherServerSettingsFile;            // Factorio launcher server settings file
        public static string factorioLauncherMapGenSettingsFile;            // Factorio launcher map gen settings file
        public static string factorioLauncherMapSettingsFile;               // Factorio launcher map settings file
        public static string factorioLauncherSettingsFile;                  // Factorio launcher server settings file

        [System.Runtime.InteropServices.DllImport("kernel32")]
        static extern int GetPrivateProfileString(string section,
        string key, string def, StringBuilder retVal,
        int size, string filePath);
        [System.Runtime.InteropServices.DllImport("kernel32")]
        static extern long WritePrivateProfileString(string section,
                string key, string val, string filePath);

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public static void Load(FormMain mainForm)
        {
            appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            programPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            programX86Path = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);

            if (GetSteamPath() != "")
            {
                steamPath = GetSteamPath();
            }
            else
            {
                steamPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + @"\Steam";
                DirectoryInfo dir = new DirectoryInfo(steamPath);
                if (!dir.Exists)
                {
                    steamPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + @"\Steam";
                    DirectoryInfo secdir = new DirectoryInfo(steamPath);
                    if (!secdir.Exists)
                    {
                        var folderBrowserDialog = new FolderBrowserDialog();
                        folderBrowserDialog.ShowNewFolderButton = false;
                        folderBrowserDialog.SelectedPath = @"C:\";
                        if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                        {
                            steamPath = folderBrowserDialog.SelectedPath.ToString();
                            ServerSettingsExample.FilePath = steamPath + @"\steamapps\common\Factorio\data\server-settings.example.json";
                            ServerSettings.FilePath = appDataPath + @"\FactorioLauncher\server-settings.json";
                        }
                    }
                }
            }

            factorioPath = appDataPath + @"\factorio\";
            factorioModsPath = factorioPath + @"mods\";
            factorioSavesPath = factorioPath + @"saves\";
            factorioConfig = factorioPath + @"config\config.ini";
            factorioInstallPath = steamPath + @"\steamapps\common\factorio\";
            factorioEXEPath = factorioInstallPath + @"bin\x64\";
            factorioEXE = factorioEXEPath + @"factorio.exe";

            factorioLauncherPath = appDataPath + @"\FactorioLauncher\";
            factorioLauncherSettingsFile = factorioLauncherPath + @"launcher-config.json";
            factorioLauncherConfig = factorioLauncherPath + @"config.ini";
            factorioLauncherServerSettingsFile = factorioLauncherPath + @"server-settings.json";
            factorioLauncherMapGenSettingsFile = factorioLauncherPath + @"map-gen-settings.json";
            factorioLauncherMapSettingsFile = factorioLauncherPath + @"map-settings.json";

            ServerSettings.FilePath = factorioLauncherServerSettingsFile;
            ServerSettingsExample.FilePath = Settings.factorioInstallPath + @"data\server-settings.example.json";

            MapGeneratorExample.FilePath = Settings.factorioInstallPath + @"data\map-gen-settings.example.json";
            MapGeneratorSettingsExample.FilePath = Settings.factorioInstallPath + @"data\map-settings.example.json";

            // Create Launcher directory
            if (!Directory.Exists(factorioLauncherPath))
            {
                DirectoryInfo NewLauncheDir = Directory.CreateDirectory(factorioLauncherPath);
            }

            if (File.Exists(factorioLauncherSettingsFile))
            {
                JObject configObject = JObject.Parse(File.ReadAllText(factorioLauncherSettingsFile));
                JObject config = (JObject)configObject;
                mainForm.showCmd.Checked = (Boolean)config["show_cmd"];
                mainForm.steamCmd.Text = (string)config["steam_path"];
            }

            WriteLog(mainForm);
        }

        public static void ReLoad(FormMain mainForm)
        {
            factorioPath = appDataPath + @"\Factorio\";
            factorioSavesPath = factorioPath + @"saves\";
            factorioInstallPath = steamPath + @"\steamapps\common\Factorio\";
            factorioEXEPath = steamPath + @"\steamapps\common\Factorio\bin\x64\";
            factorioEXE = factorioEXEPath + @"factorio.exe";
            LoadServerConfig();
        }

        public static void LoadServerConfig()
        {
            // Copy config.ini to launcher resources folder.
            if (!File.Exists(factorioLauncherConfig))
                if (Directory.Exists(factorioInstallPath) && File.Exists(factorioConfig))
                    File.Copy(factorioConfig, factorioLauncherConfig);
                else
                    MessageBox.Show("Missing " + factorioConfig + " Please start Factorio first to generate new resource files.");
            else
            {
                // Uncomment port
                string str = File.ReadAllText(factorioLauncherConfig);
                str = str.Replace("; port=34197", "port=34197");
                File.WriteAllText(factorioLauncherConfig, str);

                // Change config to allow dedicated server to run on the same machine.
                StringBuilder temp = new StringBuilder(255);
                GetPrivateProfileString("path", "write-data", factorioLauncherPath, temp, 255, factorioLauncherConfig);
                GetPrivateProfileString("other", "port", "34198", temp, 255, factorioLauncherConfig);
                String value = temp.ToString();
                WritePrivateProfileString("path", "write-data", factorioLauncherPath, factorioLauncherConfig);
                WritePrivateProfileString("other", "port", "34198", factorioLauncherConfig);
            }
        }

        public static void LoadSavedGames(FormMain mainForm)
        {
            if (Directory.Exists(Settings.factorioSavesPath))
            {
                mainForm.listBoxSavedGames.Items.Clear();
                DirectoryInfo savesDir = new DirectoryInfo(factorioSavesPath);
                var saves = savesDir.GetFiles("*.zip");
                foreach (var save in saves.Reverse())
                {
                    mainForm.listBoxSavedGames.Items.Add(save);
                }
            }
            else
            {
                DirectoryInfo NewSaveDir = Directory.CreateDirectory(factorioSavesPath);
            }
        }

        public static bool ShowCMD()
        {
            var FilePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\FactorioLauncher\launcher-config.json";
            bool showCMD;
            if (File.Exists(FilePath))        
            {
                JObject serverSettingsObject = JObject.Parse(File.ReadAllText(FilePath));
                JObject serverSettings = (JObject)serverSettingsObject;
                showCMD = ((Boolean)serverSettings["show_cmd"]); 
            }
            else
            {
                showCMD = false;
                DirectoryInfo launcherDir = Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\FactorioLauncher");
                JObject newConfig = new JObject(
                    new JProperty("show_cmd", false),
                    new JProperty("steam_path", ""));
                File.WriteAllText(FilePath, newConfig.ToString());
            }
            return showCMD;
        }

        public static void WriteLog(FormMain mainForm)
        {
            mainForm.console.AppendText("Version: " + Application.ProductVersion);

            mainForm.console.AppendText("Steam: " + steamPath);
            mainForm.console.AppendText("Factorio install: " + factorioInstallPath);
            mainForm.console.AppendText("Factorio resources: " + factorioPath);
            mainForm.console.AppendText("Factorio saves: " + factorioSavesPath);
            mainForm.console.AppendText("Factorio server config: " + factorioConfig);
            mainForm.console.AppendText("Factorio.exe: " + factorioEXE);

            mainForm.console.AppendText("Server launcher resources: " + factorioLauncherPath);
            mainForm.console.AppendText("Server launcher settings: " + factorioLauncherServerSettingsFile);
            mainForm.console.AppendText("Server launcher server config: " + factorioLauncherConfig);
            mainForm.console.AppendText("Server launcher server settings: " + factorioLauncherServerSettingsFile);

        }

        public static void SetSteamPath(string path)
        {
            JObject configObject = JObject.Parse(File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\FactorioLauncher\launcher-config.json"));
            JObject config = (JObject)configObject;
            config["steam_path"] = ((string)path);
            File.WriteAllText(factorioLauncherSettingsFile, config.ToString());
        }

        public static string GetSteamPath()
        {
            JObject configObject = JObject.Parse(File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\FactorioLauncher\launcher-config.json"));
            JObject config = (JObject)configObject;
            return config["steam_path"].ToString(); ;
        }

        public static string GetVersion(string url = "https://raw.githubusercontent.com/Limmek/Factorio-Server-Launcher/master/VERSION")
        {
            string version = (new WebClient()).DownloadString(url);
            return version;
        }

        public static bool Download;
        public static void CheckVersion(FormMain mainForm)
        {
            if (!CheckInternetConnection()) // Skip version check if no internet connection
                return;

            Version a = new Version(Application.ProductVersion.Trim());
            Version b = new Version(GetVersion().Trim());
            if (a < b)
            {
                string MessageBoxTitle = "New version available!";
                string MessageBoxContent =  "This: " + a.ToString() +
                                            "\nNew: " + b.ToString() +
                                            "\n\nDo you want to update now?";

                DialogResult dialogResult = MessageBox.Show(MessageBoxContent, MessageBoxTitle, MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                    Download = true;
            }
        }

        public static bool CheckInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                {
                    using (client.OpenRead("http://google.com"))
                    {
                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        public static void DownloadNewVersion(FormMain mainForm)
        {
            mainForm.tabControl1.TabIndex = 3;
            string url = "https://github.com/Limmek/Factorio-Server-Launcher/releases/download/" + GetVersion().Trim() + "/FactorioServerLauncher.exe";
            try
            {
                WebClient myWebClient = new WebClient();
                mainForm.console.AppendText("Downloading File FactorioServerLauncher.exe from " + url + " .......\n");
                myWebClient.DownloadFile(url, factorioLauncherPath + @"\FactorioServerLauncher.exe");
                mainForm.console.AppendText("Successfully Downloaded File FactorioServerLauncher.exe from " + url);
            }
            finally
            {
                if (File.Exists("FactorioServerLauncher.old")) File.Delete("FactorioServerLauncher.old");

                File.Move("FactorioServerLauncher.exe", "FactorioServerLauncher.old");
                File.Move(factorioLauncherPath + @"\FactorioServerLauncher.exe", "FactorioServerLauncher.exe");
                Application.Restart();
            }
        }


    }

}
