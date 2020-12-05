using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;

using CitizenFX.Core;
using CitizenFX.Core.Native;

using VinaFrameworkServer.Core;

namespace FiveM_VoiceServer.Modules
{
    public class VoiceModule : Module
    {
        public VoiceModule(Server server) : base(server)
        {
            _playerVoiceEnabled = new Dictionary<string, bool>();
            _playerVoiceLevels = new Dictionary<string, int>();
            _playerVoiceChannels = new Dictionary<string, int>();

            script.AddEvent("Voice.PlayerVoiceEnabledChanged", new Action<Player, bool>(OnPlayerVoiceEnabledChanged));
            script.AddEvent("Voice.PlayerVoiceLevelChanged", new Action<Player, int>(OnPlayerVoiceLevelChanged));
            script.AddEvent("Voice.PlayerVoiceChannelChanged", new Action<Player, int>(OnPlayerVoiceChannelChanged));

            script.SetExport("getEnabled", new Func<int, bool>(ExportGetEnabled));
            script.SetExport("setEnabled", new Action<int, bool>(ExportSetEnabled));

            script.SetExport("getVoiceLevel", new Func<int, int>(ExportGetVoiceLevel));
            script.SetExport("setVoiceLevel", new Action<int, int>(ExportSetVoiceLevel));

            script.SetExport("getVoiceChannel", new Func<int, int>(ExportGetVoiceChannel));
            script.SetExport("setVoiceChannel", new Action<int, int>(ExportSetVoiceChannel));
        }

        #region ACCESSORS



        #endregion
        #region VARIABLES

        Dictionary<string, bool> _playerVoiceEnabled;
        Dictionary<string, int> _playerVoiceLevels;
        Dictionary<string, int> _playerVoiceChannels;

        float whisper_level;
        float default_level;
        float shout_level;

        #endregion
        #region BASE EVENTS

        protected override void OnModuleInitialized()
        {
            // Print more informations
            whisper_level = (float)API.GetConvarInt("fivem_voice_level_whisper", 5);

            // Peridically print current time
            default_level = (float)API.GetConvarInt("fivem_voice_level_default", 25);

            // Console Print Time Format
            shout_level = (float)API.GetConvarInt("fivem_voice_level_shout", 50);

            Debug.WriteLine($@"
=====================================
FIVEM VOICE SETTINGS:
=====================================
 fivem_voice_level_whisper      = {whisper_level}
 fivem_voice_level_default      = {default_level}
 fivem_voice_level_shout (ms)   = {shout_level}
=====================================");
        }

        protected override void OnPlayerConnecting(Player player)
        {
            _playerVoiceEnabled.Add(player.Handle, false);
            _playerVoiceLevels.Add(player.Handle, 1);
            _playerVoiceChannels.Add(player.Handle, -1);
        }

        protected override void OnPlayerDropped(Player player, string reason)
        {
            _playerVoiceEnabled.Remove(player.Handle);
            _playerVoiceLevels.Remove(player.Handle);
            _playerVoiceChannels.Remove(player.Handle);
        }

        protected override void OnPlayerClientInitialized(Player player)
        {
            SetPlayerVoiceSettings(player.Handle);
        }

        #endregion
        #region MODULE EVENTS

        private void OnPlayerVoiceEnabledChanged([FromSource] Player player, bool enabled)
        {
            _playerVoiceEnabled[player.Handle] = enabled;
        }

        private void OnPlayerVoiceLevelChanged([FromSource] Player player, int level)
        {
            _playerVoiceLevels[player.Handle] = level;
        }

        private void OnPlayerVoiceChannelChanged([FromSource] Player player, int channel)
        {
            _playerVoiceChannels[player.Handle] = channel;
        }

        #endregion
        #region MODULE EXPORTS

        private bool ExportGetEnabled(int playerHandle)
        {
            return _playerVoiceEnabled[API.GetPlayerFromIndex(playerHandle)];
        }

        private void ExportSetEnabled(int playerHandle, bool enabled)
        {
            SetPlayerVoiceEnabled(API.GetPlayerFromIndex(playerHandle), enabled);
        }

        private int ExportGetVoiceLevel(int playerHandle)
        {
            return _playerVoiceLevels[API.GetPlayerFromIndex(playerHandle)];
        }

        private void ExportSetVoiceLevel(int playerHandle, int level)
        {
            SetPlayerVoiceLevel(API.GetPlayerFromIndex(playerHandle), level);
        }

        private int ExportGetVoiceChannel(int playerHandle)
        {
            return _playerVoiceChannels[API.GetPlayerFromIndex(playerHandle)];
        }

        private void ExportSetVoiceChannel(int playerHandle, int channel)
        {
            SetPlayerVoiceChannel(API.GetPlayerFromIndex(playerHandle), channel);
        }

        #endregion
        #region MODULE METHODS

        private void SetPlayerVoiceSettings(string player)
        {
            Server.TriggerClientEvent(player, "Voice.SetVoiceSettings", whisper_level, default_level, shout_level);
        }

        private void SetPlayerVoiceEnabled(string player, bool enabled)
        {
            _playerVoiceEnabled[player] = enabled;
            Server.TriggerClientEvent(player, "Voice.SetVoiceEnabled", enabled);
        }

        private void SetPlayerVoiceLevel(string player, int level)
        {
            _playerVoiceLevels[player] = level;
            Server.TriggerClientEvent(player, "Voice.SetVoiceLevel", level);
        }

        private void SetPlayerVoiceChannel(string player, int channel)
        {
            _playerVoiceChannels[player] = channel;
            Server.TriggerClientEvent(player, "Voice.SetVoiceChannel", channel);
        }

        #endregion
    }
}
