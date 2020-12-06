using System;
using System.Threading.Tasks;

using CitizenFX.Core;
using CitizenFX.Core.Native;

using VinaFrameworkClient.Core;

namespace Vina_VoiceClient.Modules
{
    public enum Voices
    {
        Whisper = 0,
        Default = 1,
        Shout = 2
    }

    public class VoiceModule : Module
    {
        public VoiceModule(Client client) : base(client)
        {
            script.AddEvent("Voice.SetVoiceSettings", new Action<float, float, float>(OnSetVoiceSettings));
            script.AddEvent("Voice.SetVoiceEnabled", new Action<bool>(OnSetVoiceEnabled));
            script.AddEvent("Voice.SetVoiceLevel", new Action<int>(OnSetVoiceLevel));
            script.AddEvent("Voice.SetVoiceChannel", new Action<int>(OnSetVoiceChannel));

            script.SetExport("getEnabled", new Func<bool>(ExportGetEnabled));
            script.SetExport("setEnabled", new Action<bool>(ExportSetEnabled));

            script.SetExport("getVisibility", new Func<bool>(ExportGetVisibility));
            script.SetExport("setVisibility", new Action<bool>(ExportSetVisibility));

            script.SetExport("getVoiceLevel", new Func<int>(ExportGetVoiceLevel));
            script.SetExport("setVoiceLevel", new Action<int>(ExportSetVoiceLevel));

            script.SetExport("getVoiceChannel", new Func<int>(ExportGetVoiceChannel));
            script.SetExport("setVoiceChannel", new Action<int>(ExportSetVoiceChannel));
        }

        #region MODULES

        NuiModule nuiModule;

        #endregion
        #region VARIABLES

        int playerId = -1;
        int channel = -1;
        bool enabled = false;
        bool visible = false;
        bool wasVisible = false;
        bool hidden = false;
        float levelWhisper = 5.0f;
        float levelDefault = 25.0f;
        float levelShout = 50.0f;
        Voices voiceLevel = Voices.Default;
        string voiceLevelStr = "";
        string speakingStr = "";

        #endregion
        #region BASE EVENTS

        protected override void OnModuleInitialized()
        {
            nuiModule = client.GetModule<NuiModule>();

            playerId = API.PlayerId();

            script.AddTick(ProcessControls);
            //script.AddTick(DrawVoiceLevel);
            script.AddTick(UpdateVoiceProximity);
            script.AddTick(DrawCurrentlyTalking);
        }

        #endregion
        #region MODULE EVENTS

        private void OnSetVoiceSettings(float whisper_level, float default_level, float shout_level)
        {
            script.Log($"Server SetVoiceSettings received!");

            levelWhisper = whisper_level;
            levelDefault = default_level;
            levelShout = shout_level;

            script.Log($"Whisper voice level set to {levelWhisper}");
            script.Log($"Default voice level set to {levelDefault}");
            script.Log($"Shout voice level set to {levelShout}");

            SetVoiceLevel(Voices.Default);

            SetVoiceEnabled(true);
        }

        private void OnSetVoiceEnabled(bool enabled)
        {
            SetVoiceEnabled(enabled, false);
        }

        private void OnSetVoiceLevel(int level)
        {
            SetVoiceLevel(level, false);
        }

        private void OnSetVoiceChannel(int channel)
        {
            SetVoiceChannel(channel, false);
        }

        #endregion
        #region MODULES TICKS

        private async Task UpdateVoiceProximity()
        {
            await Client.Delay(1);

            visible = isVisible();

            if (!enabled)
            {
                API.NetworkSetVoiceActive(false);
                return;
            }

            API.NetworkSetVoiceActive(true);

            if (channel < 0)
            {
                API.NetworkClearVoiceChannel();
            }
            else
            {
                API.NetworkSetVoiceChannel(channel);
            }
            
            if (voiceLevel == Voices.Whisper)
            {
                API.NetworkSetTalkerProximity(levelWhisper);
            }
            else if (voiceLevel == Voices.Default)
            {
                API.NetworkSetTalkerProximity(levelDefault);
            }
            else if (voiceLevel == Voices.Shout)
            {
                API.NetworkSetTalkerProximity(levelShout);
            }
        }

        private async Task ProcessControls()
        {
            if (!enabled) return;

            bool keyOne = (Game.IsControlPressed(0, Control.Sprint) || Game.IsDisabledControlPressed(0, Control.Sprint));
            
            if (keyOne)
            {
                bool keyTwo = (Game.IsControlJustPressed(0, Control.VehicleHeadlight) || Game.IsDisabledControlJustPressed(0, Control.VehicleHeadlight));

                if (keyTwo)
                {
                    SetNextVoiceLevel();
                }
            }
        }

        private async Task DrawCurrentlyTalking()
        {
            await Client.Delay(100);

            if (!enabled || !visible) return;

            string talkingString = "";

            foreach (Player player in client.GetPlayers())
            {
                if (API.NetworkIsPlayerTalking(player.Handle))
                {
                    if (player.Handle == playerId)
                    {
                        talkingString += "<span class='self'>You</span>";
                    }
                    else
                    {
                        talkingString += $"<span>{player.Name}</span>";
                    }
                }
            }

            if (speakingStr != talkingString)
            {
                speakingStr = talkingString;

                nuiModule.UpdateTalking(speakingStr);
            }

            talkingString = null;
        }

        #endregion
        #region MODULES METHODS

        private bool isVisible()
        {
            bool _visible = (!hidden && enabled && API.IsHudPreferenceSwitchedOn() && !API.IsPlayerSwitchInProgress() && API.IsScreenFadedIn() && !API.IsPauseMenuActive() && !API.IsFrontendFading() && !API.IsPauseMenuRestarting());
            
            if (!wasVisible && _visible)
            {
                nuiModule.SetVisibility(true);
                wasVisible = true;
            }
            else if (wasVisible && !_visible)
            {
                nuiModule.SetVisibility(false);
                wasVisible = false;
            }

            return _visible;
        }

        private void SetVoiceEnabled(bool isEnabled, bool net = true)
        {
            enabled = isEnabled;
            nuiModule.SetEnabled(enabled);
            if (net) Client.TriggerServerEvent("Voice.PlayerVoiceEnabledChanged", enabled);
        }

        private void SetVoiceLevel(int level, bool net = true)
        {
            if (level >= 0 && level <= 2)
            {
                SetVoiceLevel((Voices)level, net);
            }
            else SetVoiceLevel(Voices.Default, net);
        }
        private void SetVoiceLevel(Voices level, bool net = true)
        {
            voiceLevel = level;

            if (voiceLevel == Voices.Whisper)
            {
                voiceLevelStr = $"Whisper ({levelWhisper}m)";
            }
            else if (voiceLevel == Voices.Default)
            {
                voiceLevelStr = $"Default ({levelDefault}m)";
            }
            else if (voiceLevel == Voices.Shout)
            {
                voiceLevelStr = $"Shout ({levelShout}m)";
            }

            nuiModule.SetVoiceLevel(voiceLevelStr);

            if (net) Client.TriggerServerEvent("Voice.PlayerVoiceLevelChanged", (int) voiceLevel);

            script.Log($"Voice level set to {voiceLevelStr}");
        }

        private void SetVoiceChannel(int voiceChannel, bool net = true)
        {
            if (voiceChannel < -1) voiceChannel = -1;
            channel = voiceChannel;

            nuiModule.SetChannel(channel);

            if (net) Client.TriggerServerEvent("Voice.PlayerVoiceChannelChanged", channel);
        }

        private void SetNextVoiceLevel()
        {
            if (voiceLevel == Voices.Whisper)
            {
                voiceLevel = Voices.Default;
            }
            else if (voiceLevel == Voices.Default)
            {
                voiceLevel = Voices.Shout;
            }
            else if (voiceLevel == Voices.Shout)
            {
                voiceLevel = Voices.Whisper;
            }

            SetVoiceLevel(voiceLevel);
        }

        #endregion
        #region MODULE EXPORTS

        private bool ExportGetEnabled()
        {
            return enabled;
        }

        private void ExportSetEnabled(bool isEnabled)
        {
            SetVoiceEnabled(isEnabled);
            script.Log($"{((enabled) ? "Enabled" : "Disabled")} from an export call.");
        }

        private bool ExportGetVisibility()
        {
            return visible;
        }

        private void ExportSetVisibility(bool isVisible)
        {
            hidden = !isVisible;
            script.Log($"{((hidden) ? "Hidden" : "Shown")} from an export call.");
        }

        private int ExportGetVoiceLevel()
        {
            return (int)voiceLevel;
        }

        private void ExportSetVoiceLevel(int voiceLevel)
        {
            SetVoiceLevel((Voices)voiceLevel);
            script.Log($"Voice level set to {voiceLevel} from an export call.");
        }

        private int ExportGetVoiceChannel()
        {
            return channel;
        }

        private void ExportSetVoiceChannel(int voiceChannel)
        {
            SetVoiceChannel(voiceChannel);
            script.Log($"Voice channel set to {channel} from an export call.");
        }

        #endregion
    }
}
