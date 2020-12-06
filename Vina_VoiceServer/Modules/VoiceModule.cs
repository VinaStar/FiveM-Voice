using System;
using System.Collections.Generic;

using CitizenFX.Core;
using CitizenFX.Core.Native;

using VinaFrameworkServer.Core;

namespace Vina_VoiceServer.Modules
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

            script.SetExport("getEnabled", new Func<string, bool>(ExportGetEnabled));
            script.SetExport("setEnabled", new Action<string, bool>(ExportSetEnabled));

            script.SetExport("getVoiceLevel", new Func<string, int>(ExportGetVoiceLevel));
            script.SetExport("setVoiceLevel", new Action<string, int>(ExportSetVoiceLevel));

            script.SetExport("getVoiceChannel", new Func<string, int>(ExportGetVoiceChannel));
            script.SetExport("setVoiceChannel", new Action<string, int>(ExportSetVoiceChannel));
        }

        #region VARIABLES

        Dictionary<string, bool> _playerVoiceEnabled;
        Dictionary<string, int> _playerVoiceLevels;
        Dictionary<string, int> _playerVoiceChannels;

        float whisper_proximity_distance;
        float default_proximity_distance;
        float shout_proximity_distance;

        #endregion
        #region BASE EVENTS

        protected override void OnModuleInitialized()
        {
            // Print more informations
            whisper_proximity_distance = (float)API.GetConvarInt("fivem_voice_whisper_proximity_distance", 5);

            // Peridically print current time
            default_proximity_distance = (float)API.GetConvarInt("fivem_voice_default_proximity_distance", 25);

            // Console Print Time Format
            shout_proximity_distance = (float)API.GetConvarInt("fivem_voice_shout_proximity_distance", 50);

            Debug.WriteLine($@"
=====================================
FIVEM VOICE SETTINGS:
=====================================
 fivem_voice_level_whisper      = {whisper_proximity_distance}
 fivem_voice_level_default      = {default_proximity_distance}
 fivem_voice_level_shout (ms)   = {shout_proximity_distance}
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
            SetPlayerVoiceSettings(player);
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

        private bool ExportGetEnabled(string playerHandle)
        {
            return _playerVoiceEnabled[playerHandle];
        }

        private void ExportSetEnabled(string playerHandle, bool enabled)
        {
            SetPlayerVoiceEnabled(playerHandle, enabled);
        }

        private int ExportGetVoiceLevel(string playerHandle)
        {
            return _playerVoiceLevels[playerHandle];
        }

        private void ExportSetVoiceLevel(string playerHandle, int level)
        {
            SetPlayerVoiceLevel(playerHandle, level);
        }

        private int ExportGetVoiceChannel(string playerHandle)
        {
            return _playerVoiceChannels[playerHandle];
        }

        private void ExportSetVoiceChannel(string playerHandle, int channel)
        {
            SetPlayerVoiceChannel(playerHandle, channel);
        }

        #endregion
        #region MODULE METHODS

        private void SetPlayerVoiceSettings(Player player)
        {
            Server.TriggerClientEvent(player, "Voice.SetVoiceSettings", whisper_proximity_distance, default_proximity_distance, shout_proximity_distance);
        }

        private void SetPlayerVoiceEnabled(string playerHandle, bool enabled)
        {
            _playerVoiceEnabled[playerHandle] = enabled;
            Server.TriggerClientEvent(playerHandle, "Voice.SetVoiceEnabled", enabled);
        }

        private void SetPlayerVoiceLevel(string playerHandle, int level)
        {
            _playerVoiceLevels[playerHandle] = level;
            Server.TriggerClientEvent(playerHandle, "Voice.SetVoiceLevel", level);
        }

        private void SetPlayerVoiceChannel(string playerHandle, int channel)
        {
            _playerVoiceChannels[playerHandle] = channel;
            Server.TriggerClientEvent(playerHandle, "Voice.SetVoiceChannel", channel);
        }

        #endregion
    }
}
