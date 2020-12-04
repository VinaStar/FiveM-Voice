using System;
using System.IO;
using System.Threading.Tasks;

using CitizenFX.Core;
using CitizenFX.Core.Native;

using VinaFrameworkServer.Core;

namespace FiveM_VoiceServer.Modules
{
    public class VoiceModule : Module
    {
        public VoiceModule(Server server) : base(server)
        {
            
        }

        #region ACCESSORS



        #endregion
        #region VARIABLES

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

        }

        protected override void OnPlayerDropped(Player player, string reason)
        {

        }

        protected override void OnPlayerClientInitialized(Player player)
        {
            UpdatePlayerVoiceLevels(player);
        }

        #endregion
        #region MODULE TICKS



        #endregion
        #region MODULE METHODS

        private void UpdatePlayerVoiceLevels(Player player = null)
        {
            if (player != null)
            {
                script.Log($"Sending SetVoiceSettings to player {player.Name}");
                Server.TriggerClientEvent(player, "Voice.SetVoiceSettings", whisper_level, default_level, shout_level);
            }
        }

        #endregion
    }
}
