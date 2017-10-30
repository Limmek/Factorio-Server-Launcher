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
        public static void Start(string Path, string Username="", string Password="")
        {
            string AnonumysLogin = "";
            if (Username != "" && Password != "")
                AnonumysLogin = "+login " + Username + " " + Password;

            var StartArguments = AnonumysLogin + " +force_install_dir " + Settings.factorioInstallPath + " +app_update 427520 validate +quit";
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.WindowStyle = ProcessWindowStyle.Normal;
            startInfo.Verb = "runas";

            startInfo.FileName = Path + @"\steamcmd.exe";
            startInfo.Arguments = StartArguments;
            startInfo.WorkingDirectory = Path;
            startInfo.CreateNoWindow = false;
            startInfo.RedirectStandardInput = false;
            startInfo.RedirectStandardOutput = false;
            startInfo.UseShellExecute = true;

            DownloadFactorio = Process.Start(startInfo);
        }

    }
}
