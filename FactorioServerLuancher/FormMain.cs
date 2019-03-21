using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Reflection;
using System.Net;

namespace FactorioServerLauncher
{
    public partial class FormMain : Form
    {
        [DllImport("kernel32.dll", EntryPoint = "GetStdHandle", SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr GetCmdHandle(int nStdHandle);

        [DllImport("kernel32.dll", EntryPoint = "AllocConsole", SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int AllocConsole();

        private const int CMD_OUTPUT_HANDLE = -11;
        private const int MY_CODE_PAGE = 437;
        
        private static bool showFactorioLauncherConsole = Settings.ShowCMD();
        public static bool Loaded;
        public static bool LoadedB;

        public FormMain()
        {
            InitializeComponent();

            Settings.Load(this);
            Settings.LoadSavedGames(this);

            if (showFactorioLauncherConsole)
            {
                AllocConsole();
                IntPtr handle = GetCmdHandle(CMD_OUTPUT_HANDLE);
                Microsoft.Win32.SafeHandles.SafeFileHandle safeFileHandle = new Microsoft.Win32.SafeHandles.SafeFileHandle(handle, true);
                FileStream fileStream = new FileStream(safeFileHandle, FileAccess.Write);
                Encoding encoding = Encoding.GetEncoding(MY_CODE_PAGE);
                StreamWriter standardOutput = new StreamWriter(fileStream, encoding);
                standardOutput.AutoFlush = true;
                Console.SetOut(standardOutput);
            }

            if (!File.Exists(ServerSettings.FilePath)) bSaveSettings.Enabled = true;
        }

        private void settingsTagsList_DoubleClick(object sender, EventArgs e)
        {
            ServerSettings.RemoveTag(this);
            bLoadSettings.PerformClick();
        }

        private void settingsTagsList_Click(object sender, EventArgs e)
        {
            ServerSettings.tagId = settingsTagsList.SelectedIndex.ToString();
        }

        private void settingsAdminsList_DoubleClick(object sender, EventArgs e)
        {
            ServerSettings.RemoveAdmin(this);
            bLoadSettings.PerformClick();
        }

        private void settingsAdminsList_Click(object sender, EventArgs e)
        {
            ServerSettings.adminId = settingsAdminsList.SelectedIndex.ToString();
        }

        private void mLoadExample_Click(object sender, EventArgs e)
        {
            // Clear Boxes
            settingsTagsList.Items.Clear();
            settingsAdminsList.Items.Clear();
            console.Clear();

            // Enable buttons
            bAddTag.Enabled = true;
            bAddAdmin.Enabled = true;
            bSaveSettings.Enabled = true;

            // Load server-settings-example.json
            ServerSettingsExample.LoadSettingsExample(this);
        }

        private void bLoadSettings_Click(object sender, EventArgs e)
        {
            Loaded = true;
            // Clear Boxes
            settingsTagsList.Items.Clear();
            settingsAdminsList.Items.Clear();
            console.Clear();

            // Enable buttons
            bAddTag.Enabled = true;
            bAddAdmin.Enabled = true;
            bSaveSettings.Enabled = true;
            
            // Load server-settings.json
            ServerSettings.LoadSettings(this);    
        }

        private void bSaveSettings_Click(object sender, EventArgs e)
        {
            console.Clear();
            ServerSettings.SaveSettings(this); // Save server-settings.json
            Settings.LoadServerConfig();
        }

        private void bAddTag_Click(object sender, EventArgs e)
        {
            ServerSettings.AddTag(this, settingsTag.Text);
            bLoadSettings.PerformClick();
        }

        private void bAddAdmin_Click(object sender, EventArgs e)
        {
            ServerSettings.AddAdmin(this, settingsAdmin.Text);
            bLoadSettings.PerformClick();
        }

        private void bStartServer_Click(object sender, EventArgs e)
        {
            
            string Title = "Factorio Server Launcher";
            Settings.LoadServerConfig();
            if (StartServer.ServerId == 0)
            {
                tReloadSavedGames.Enabled = true;
                console.Clear();
                StartServer.Start(this);
                bStartServer.Text = "Stop Server";
                console.AppendText(Environment.NewLine + "Starting server...");
                console.AppendText(Environment.NewLine + Settings.factorioEXE + StartServer.StartArguments);
                runTimeTimer.Enabled = true;
                lStatus.Text = "Running";
                this.Text = Title + " - Running";
                bStartServer.BackColor = System.Drawing.Color.IndianRed;
            }
            else
            {
                tReloadSavedGames.Enabled = false;
                StartServer.stop();
                bStartServer.Text = "Start Server";
                console.AppendText(Environment.NewLine + "Closing server...");
                runTimeTimer.Enabled = false;
                lStatus.Text = "Stopped   Run time:";
                this.Text = Title;
                bStartServer.BackColor = System.Drawing.Color.LightGreen;
            }
        }

        private void mSteam_Click(object sender, EventArgs e)
        {
            var folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.ShowNewFolderButton = false;
            folderBrowserDialog.SelectedPath = @"C:\";
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                Settings.steamPath = folderBrowserDialog.SelectedPath.ToString();
                Settings.SetSteamPath(Settings.steamPath);
                lSteam.Text = "Steam Path: " + Settings.steamPath;
                steamCmd.Text = Settings.steamPath;
                Settings.ReLoad(this);
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://factorio.com/login");
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://steamcommunity.com/dev/managegameservers");
        }

        private void listBoxSavedGames_Click(object sender, EventArgs e)
        {
            tReloadSavedGames.Enabled = false;

            if ((!bStartServer.Enabled) && (!Loaded))
            {
                MessageBox.Show("Load server settings first!");
                Loaded = false;
                return;
            }

            if (listBoxSavedGames.Items.Count >= 1 && StartServer.ServerId == 0)
            {
                bStartServer.Enabled = true;
                bStartServer.BackColor = Color.LightGreen;
            }

        }

        private void showCmd_CheckedChanged(object sender, EventArgs e)
        {
            console.AppendText(Environment.NewLine + "show_cmd: " + ((CheckBox)sender).Checked.ToString());

            JObject configObject = JObject.Parse(File.ReadAllText(Settings.factorioLauncherSettingsFile));
            JObject config = (JObject)configObject;
            config["show_cmd"] = ((CheckBox)sender).Checked;
            File.WriteAllText(Settings.factorioLauncherSettingsFile, config.ToString());
        }

        private void showCmd_Click(object sender, EventArgs e)
        {
            string MessageBoxTitle = "Show CMD";
            string MessageBoxContent = "Need to restart the " + FormMain.ActiveForm.Text + " for this action to take effect.\nDo you want to restart now?";

            DialogResult dialogResult = MessageBox.Show(MessageBoxContent, MessageBoxTitle, MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
                Application.Restart();
        }

        private void openLauncherFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("explorer.exe", Settings.factorioLauncherPath);
        }

        private void bOpenFactorioFolder_Click(object sender, EventArgs e)
        {
            Process.Start("explorer.exe", Settings.factorioPath);
        }

        private void bOpenInstallFolder_Click(object sender, EventArgs e)
        {
            Process.Start("explorer.exe", Settings.factorioInstallPath);
        }

        private void bOpenSteamFolder_Click(object sender, EventArgs e)
        {
            Process.Start("explorer.exe", Settings.steamPath);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            TimeSpan runtime;
            runtime = DateTime.Now - StartServer.FactorioDedicatedServer.StartTime;
            string runtimeFixed = new DateTime(runtime.Ticks).ToString("HH:mm:ss");
            lTime.Text = runtimeFixed;
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.saveSteamSettings(Settings.steamPath, Settings.betaOptIn, Settings.betaVersion, Settings.steamUsername);
            StartServer.stop();
        }

        private void bInstallFactorio_Click(object sender, EventArgs e)
        {
            if (!File.Exists(steamCmd.Text + @"\steamcmd.exe"))
            {
                MessageBox.Show("Steam CMD not found!");
                return;
            }
            Settings.betaOptIn = steamBetaOptIn.Checked;
            Settings.betaVersion = steamBetaVersion.Text;
            Settings.steamUsername = steamUsername.Text;

            Download.Start(steamCmd.Text, 
                steamUsername.Text, 
                steamPassword.Text, 
                steamTwoFactor.Text,
                steamBetaOptIn.Checked, 
                steamBetaVersion.Text);
        }

        private void bSteamCmd_Click(object sender, EventArgs e)
        {
            var folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.ShowNewFolderButton = false;
            folderBrowserDialog.SelectedPath = @"C:\";
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                steamCmd.Text = folderBrowserDialog.SelectedPath.ToString();
                lInstallFactorio.Text = Settings.factorioInstallPath;
            }
        }

        private void console_TextChanged(object sender, EventArgs e)
        {
            console.Text += Environment.NewLine;
        }

        private void FormMain_Activated(object sender, EventArgs e)
        {
            Settings.LoadSavedGames(this);
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            Settings.CheckVersion(this);
            steamCmd.Text = Settings.steamPath;
            lSteam.Text = "Steam Path: " + Settings.steamPath;
            lInstallFactorio.Text = Settings.factorioInstallPath;
            if (File.Exists("FactorioServerLauncher.old")) File.Delete("FactorioServerLauncher.old");
        }

        private void bDownloadSteamCMD_Click(object sender, EventArgs e)
        {
            if (!Settings.CheckInternetConnection())
            {
                MessageBox.Show("No internet connection!");
                return;
            }

            string path = steamCmd.Text + @"\steamcmd.zip";

            if (File.Exists(path))
            {
                MessageBox.Show(path + " exists already!");
                return;
            }

            try
            {
                string url = "https://steamcdn-a.akamaihd.net/client/installer/steamcmd.zip";

                console.AppendText(Environment.NewLine + "Downloading: " + url);

                var steamCMD = new WebClient();
                steamCMD.DownloadFile(url, path);
            }
            finally
            {
                ZipFile.ExtractToDirectory(path, steamCmd.Text);
                console.AppendText(Environment.NewLine + "Unziping to: " + path);
            }            
        }

        private void mapGeneratorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bGenerateMap.Enabled = true;
            bGenerateMap.Text = "Generate";
            console.Clear();
            MapGeneratorExample.LoadExample(this);
            MapGeneratorSettingsExample.LoadExample(this);
        }

        private void tReloadSavedGames_Tick(object sender, EventArgs e)
        {
            Settings.LoadSavedGames(this);
        }

        private void bGenerateMap_Click(object sender, EventArgs e)
        {
            tReloadSavedGames.Enabled = true;
            try
            {
                MapGeneratorSettings.LoadExample(this);
                MapGenerator.SaveSettings(this);
            }
            finally
            {
                MapGenerator.Generate(mapGeneratorName.Text);
            }
        }

        private void FormMain_Shown(object sender, EventArgs e)
        {
            if (Settings.Download)
                Settings.DownloadNewVersion(this);
        }

        private void modEnabled_CheckedChanged(object sender, EventArgs e)
        {
            if (modEnabled.Checked)
                modEnabled.Text = "Mods Enabled";
            else
                modEnabled.Text = "Mods Disabled";
        }
    }
}
