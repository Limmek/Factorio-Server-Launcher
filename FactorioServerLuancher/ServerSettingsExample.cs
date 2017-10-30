using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO.Compression;
using System.Windows.Forms;
using System.ComponentModel;

namespace FactorioServerLauncher
{
    public class ServerSettingsExample
    {
        public static string FilePath;

        public static void LoadSettingsExample(FormMain mainForm)
        {
            JObject serverSettingsExampleObject = JObject.Parse(File.ReadAllText(FilePath));
            JObject serverSettings = (JObject)serverSettingsExampleObject;

            mainForm.settingsName.Text = serverSettings["name"].ToString();
            mainForm.settingsDescription.Text = serverSettings["description"].ToString();
            var serverSettingList = serverSettings["tags"].ToList();
            foreach (string value in serverSettingList)
            {
                mainForm.settingsTagsList.Items.Add(value);
            }

            mainForm.settingsMaxPlayers.Text = ((int)serverSettings["max_players"]).ToString();

            mainForm.settingsPublic.Checked = ((Boolean)serverSettings["visibility"]["public"]);
            mainForm.settingsLan.Checked = ((Boolean)serverSettings["visibility"]["lan"]);

            mainForm.settingsUsername.Text = ((string)serverSettings["username"].ToString());
            mainForm.settingsPassword.Text = ((string)serverSettings["password"].ToString());
            mainForm.settingsToken.Text = ((string)serverSettings["token"].ToString());

            mainForm.settingsUserVerification.Checked = ((Boolean)serverSettings["require_user_verification"]);

            mainForm.settingsMaxUpload.Text = ((string)serverSettings["max_upload_in_kilobytes_per_second"].ToString());
            mainForm.settingsMinLatency.Text = ((string)serverSettings["minimum_latency_in_ticks"].ToString());

            mainForm.settingsIgnorePlayerLimit.Checked = ((Boolean)serverSettings["ignore_player_limit_for_returning_players"]);
            mainForm.settingsAllowCommands.Text = ((string)serverSettings["allow_commands"].ToString());

            mainForm.settingsAutoSaveInterval.Text = ((string)serverSettings["autosave_interval"].ToString());
            mainForm.settingsAutoSaveSlots.Text = ((string)serverSettings["autosave_slots"].ToString());
            mainForm.settingsAutoKick.Text = ((string)serverSettings["afk_autokick_interval"].ToString());
            mainForm.settingsAutoPause.Checked = ((Boolean)serverSettings["auto_pause"]);

            mainForm.settingsOnlyAdminsCanPause.Checked = ((Boolean)serverSettings["only_admins_can_pause_the_game"]);

            mainForm.settingsAutoSaveOnlyOnServer.Checked = ((Boolean)serverSettings["autosave_only_on_server"]);

            mainForm.settingsGamePassword.Text = ((string)serverSettings["game_password"]);

            mainForm.console.AppendText(FilePath);
            mainForm.console.AppendText(serverSettings.ToString());
        }

    }
}
