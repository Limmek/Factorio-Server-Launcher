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
using System.Diagnostics;

namespace FactorioServerLauncher
{
    class Download
    {
        public static Process DownloadFactorio = new Process();

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public static void Start(string Path, string Username = "", string Password = "")
        {
            string tempPath = System.IO.Path.GetTempPath();
            string AnonumysLogin = "";
            if (Username != "" && Password != "")
                AnonumysLogin = "login " + Username + " " + Password;

            CreateSteamCmdScript(AnonumysLogin);

            var StartArguments = "+runscript " + tempPath + "steamcmd.txt";
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                WindowStyle = ProcessWindowStyle.Normal,
                Verb = "runas",

                FileName = Path + @"\steamcmd.exe",
                Arguments = StartArguments,
                WorkingDirectory = Path,
                CreateNoWindow = false,
                RedirectStandardInput = false,
                RedirectStandardOutput = false,
                UseShellExecute = true
            };

            DownloadFactorio = Process.Start(startInfo);
            DownloadFactorio.WaitForExit();

            if (File.Exists(tempPath + "steamcmd.txt"))
            {
                File.Delete(tempPath + "steamcmd.txt");
            }
        }

        private static void CreateSteamCmdScript(string login)
        {
            string[] scriptLines = {
                login,
                "force_install_dir \"" + Settings.factorioInstallPath + "\"",
                "app_update "+ Settings.appId.ToString(),
                "quit"
            };

            string tempPath = Path.GetTempPath();

            System.IO.File.WriteAllLines(tempPath + "steamcmd.txt", scriptLines);
        }

    }
}
