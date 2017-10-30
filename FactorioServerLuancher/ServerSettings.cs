using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FactorioServerLauncher
{
    class ServerSettings
    {
        public static string FilePath; // Setts in settings
        public static string NewFilePath;
        public static string tagId;
        public static string adminId;

        public static void LoadSettings(FormMain mainForm)
        {
            if (!File.Exists(Settings.factorioLauncherServerSettingsFile))
                mainForm.mLoadExample.PerformClick();   // Generate from example file if no settings file is found
            else
            {
                JObject serverSettingsObject = JObject.Parse(File.ReadAllText(FilePath));
                JObject serverSettings = (JObject)serverSettingsObject;

                mainForm.settingsName.Text = serverSettings["name"].ToString();
                mainForm.settingsDescription.Text = serverSettings["description"].ToString();
                var tags = serverSettings["tags"].ToList();
                foreach (string tag in tags)
                {
                    mainForm.settingsTagsList.Items.Add(tag);
                }

                mainForm.settingsMaxPlayers.Text = ((int)serverSettings["max_players"]).ToString();

                mainForm.settingsPublic.Checked = ((Boolean)serverSettings["visibility"]["public"]);
                mainForm.settingsLan.Checked = ((Boolean)serverSettings["visibility"]["lan"]);

                mainForm.settingsUsername.Text = ((string)serverSettings["username"].ToString());
                mainForm.settingsPassword.Text = ((string)serverSettings["password"].ToString());
                mainForm.settingsToken.Text = ((string)serverSettings["token"].ToString());

                mainForm.settingsGamePassword.Text = ((string)serverSettings["game_password"]);
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

                var admins = serverSettings["admins"].ToList();
                foreach (string admin in admins)
                {
                    mainForm.settingsAdminsList.Items.Add(admin);
                }

                mainForm.console.AppendText(FilePath);
                mainForm.console.AppendText(serverSettings.ToString());
                }
        }

        public static void SaveSettings(FormMain mainForm)
        {
            NewFilePath = Settings.factorioLauncherServerSettingsFile;

            FileInfo dir = new FileInfo(FilePath);
            if (!dir.Exists)
                FilePath = ServerSettingsExample.FilePath;

            JObject serverSettingsObject = JObject.Parse(File.ReadAllText(FilePath));
            JObject serverSettings = (JObject)serverSettingsObject;

            serverSettings["name"] = mainForm.settingsName.Text;
            serverSettings["description"] = mainForm.settingsDescription.Text;
            JArray tags = (JArray)serverSettings["tags"];
            serverSettings["max_players"] = Convert.ToInt32(mainForm.settingsMaxPlayers.Text);

            serverSettings["visibility"]["public"] = mainForm.settingsPublic.Checked;
            serverSettings["visibility"]["lan"] = mainForm.settingsLan.Checked;            
            serverSettings["username"] = mainForm.settingsUsername.Text;
            serverSettings["password"] = mainForm.settingsPassword.Text;
            serverSettings["token"] = mainForm.settingsToken.Text;

            serverSettings["game_password"] = mainForm.settingsGamePassword.Text;
            serverSettings["require_user_verification"] = mainForm.settingsUserVerification.Checked;

            serverSettings["max_upload_in_kilobytes_per_second"] = mainForm.settingsMaxUpload.Text;
            serverSettings["minimum_latency_in_ticks"] = mainForm.settingsMinLatency.Text;

            serverSettings["ignore_player_limit_for_returning_players"] = mainForm.settingsIgnorePlayerLimit.Checked;

            if (mainForm.settingsAllowCommands.Text == "false")
                serverSettings["allow_commands"] = Convert.ToBoolean(mainForm.settingsAllowCommands.Text);
            if (mainForm.settingsAllowCommands.Text != "admins-only")
                serverSettings["allow_commands"] = Convert.ToBoolean(mainForm.settingsAllowCommands.Text);
            else
                serverSettings["allow_commands"] = mainForm.settingsAllowCommands.Text;
            
            serverSettings["autosave_interval"] = mainForm.settingsAutoSaveInterval.Text;
            serverSettings["autosave_slots"] = mainForm.settingsAutoSaveSlots.Text;
            serverSettings["afk_autokick_interval"] = mainForm.settingsAutoKick.Text;
            serverSettings["auto_pause"] = mainForm.settingsAutoPause.Checked;

            serverSettings["only_admins_can_pause_the_game"] = mainForm.settingsOnlyAdminsCanPause.Checked;

            serverSettings["autosave_only_on_server"] = mainForm.settingsAutoSaveOnlyOnServer.Checked;

            JArray admins = (JArray)serverSettings["admins"];

            mainForm.console.AppendText(FilePath);
            mainForm.console.AppendText(serverSettings.ToString());

            if (dir.Exists)
                File.WriteAllText(FilePath, serverSettings.ToString());
            else
                File.WriteAllText(NewFilePath, serverSettings.ToString());
        }

        public static void AddAdmin(FormMain mainForm, string name)
        {
            if (File.Exists(ServerSettings.FilePath))
            {
                mainForm.settingsAdminsList.Items.Add(mainForm.settingsAdmin.Text);

                JObject serverSettingsObject = JObject.Parse(File.ReadAllText(ServerSettings.FilePath));
                JObject serverSettings = (JObject)serverSettingsObject;
                JArray admin = (JArray)serverSettings["admins"];
                admin.Add(name);
                File.WriteAllText(ServerSettings.FilePath, serverSettings.ToString());
            }
            else
                MessageBox.Show("Save settings first!");

            mainForm.settingsAdmin.Text = "";
        }

        public static void AddTag(FormMain mainForm, string name)
        {
            if (File.Exists(ServerSettings.FilePath))
            {
                mainForm.settingsTagsList.Items.Add(mainForm.settingsTag.Text);

                JObject serverSettingsObject = JObject.Parse(File.ReadAllText(ServerSettings.FilePath));
                JObject serverSettings = (JObject)serverSettingsObject;
                JArray tag = (JArray)serverSettings["tags"];
                tag.Add(name);
                File.WriteAllText(ServerSettings.FilePath, serverSettings.ToString());
            }
            else
                MessageBox.Show("Save settings first!");

            mainForm.settingsTag.Text = "";
        }

        public static void RemoveAdmin(FormMain mainForm)
        {
            if (mainForm.settingsAdminsList.SelectedItems.Count != 0)
            {
                while (mainForm.settingsAdminsList.SelectedIndex != -1)
                {
                    mainForm.settingsAdminsList.Items.RemoveAt(mainForm.settingsAdminsList.SelectedIndex);
                    if (ServerSettings.FilePath != null)
                    {
                        JObject serverSettings = JObject.Parse(File.ReadAllText(ServerSettings.FilePath));
                        JObject channel = (JObject)serverSettings;
                        if ((channel["admins"].Count() != 0))
                            (channel["admins"] as JArray).RemoveAt(Convert.ToInt32(adminId));

                        mainForm.console.AppendText(serverSettings.ToString());
                        File.WriteAllText(ServerSettings.FilePath, serverSettings.ToString());
                    }
                }
            }
        }

        public static void RemoveTag(FormMain mainForm)
        {
            if (mainForm.settingsTagsList.SelectedItems.Count != 0)
            {
                while (mainForm.settingsTagsList.SelectedIndex != -1)
                {
                    mainForm.settingsTagsList.Items.RemoveAt(mainForm.settingsTagsList.SelectedIndex);
                    if (ServerSettings.FilePath != null)
                    {
                        JObject serverSettings = JObject.Parse(File.ReadAllText(ServerSettings.FilePath));
                        JObject channel = (JObject)serverSettings;
                        if ((channel["tags"].Count() != 0))
                            (channel["tags"] as JArray).RemoveAt(Convert.ToInt32(tagId));

                        mainForm.console.AppendText(serverSettings.ToString());
                        File.WriteAllText(ServerSettings.FilePath, serverSettings.ToString());
                    }
                }
            }
        }

    }
}
