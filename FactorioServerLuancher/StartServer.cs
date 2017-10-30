using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Permissions;
using System.Windows.Forms;
using System.Threading;

namespace FactorioServerLauncher
{
    class StartServer
    {
        public static int ServerId = 0;
        public static string StartArguments;
        public static Process FactorioDedicatedServer = new Process();

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public static void Start(FormMain mainForm)
        {
            StartArguments =    " --start-server " + Settings.factorioSavesPath + mainForm.listBoxSavedGames.SelectedItem.ToString() +
                                " --server-settings " + Settings.factorioLauncherServerSettingsFile +
                                " -c " + Settings.factorioLauncherConfig;

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.WindowStyle = ProcessWindowStyle.Normal;
            startInfo.Verb = "runas";
            startInfo.FileName = Settings.factorioEXE;
            startInfo.Arguments = StartArguments;
            startInfo.WorkingDirectory = Path.GetDirectoryName(Settings.factorioEXE);
            startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = true;

            FactorioDedicatedServer = Process.Start(startInfo);
            ServerId = FactorioDedicatedServer.Id;
            Console.WriteLine(StartArguments);
        }

        public static void stop()
        {
            if (ServerId != 0)
                FactorioDedicatedServer.Kill();
            ServerId = 0;
        }

    }

}
