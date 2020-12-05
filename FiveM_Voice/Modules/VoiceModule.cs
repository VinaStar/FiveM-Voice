using System;
using System.Threading.Tasks;

using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using VinaFrameworkClient.Core;

namespace FiveM_Voice.Modules
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

        #region VARIABLES

        int playerId = -1;
        int channel = -1;
        bool enabled = false;
        bool visible = false;
        bool hidden = false;
        float levelWhisper = 5.0f;
        float levelDefault = 25.0f;
        float levelShout = 50.0f;
        Voices voiceLevel = Voices.Default;
        string voiceLevelStr = "";

        #endregion
        #region BASE EVENTS

        protected override void OnModuleInitialized()
        {
            playerId = API.PlayerId();

            script.AddTick(ProcessControls);
            script.AddTick(DrawVoiceLevel);
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
            bool keyTwo = (Game.IsControlJustPressed(0, Control.VehicleHeadlight) || Game.IsDisabledControlJustPressed(0, Control.VehicleHeadlight));

            if (keyOne && keyTwo)
            {
                SetNextVoiceLevel();
            }
        }

        private async Task DrawVoiceLevel()
        {
            if (!enabled || !visible) return;

            if (API.NetworkIsPlayerTalking(playerId))
            {
                DrawLevel(41, 128, 185, 255);
            }
            else
            {
                DrawLevel(185, 185, 185, 255);
            }
        }

        private async Task DrawCurrentlyTalking()
        {
            var i = 1;
            var currentlyTalking = false;

            if (!enabled || !visible) return;

            foreach (Player player in client.GetPlayers())
            {
                if (API.NetworkIsPlayerTalking(player.Handle))
                {
                    if (!currentlyTalking)
                    {
                        DrawTextOnScreen("~s~Currently Talking", 0.5f, 0.00f, 0.5f, Alignment.Center, 6, false);
                        currentlyTalking = true;
                    }
                    if (player.Handle == playerId)
                    {
                        DrawTextOnScreen($"~b~You", 0.5f, 0.00f + (i * 0.03f), 0.5f, Alignment.Center, 6, false);
                    }
                    else
                    {
                        DrawTextOnScreen($"~b~{player.Name}", 0.5f, 0.00f + (i * 0.03f), 0.5f, Alignment.Center, 6, false);
                    }
                    i++;
                }
            }
        }

        #endregion
        #region MODULES METHODS

        private bool isVisible()
        {
            return (!hidden && enabled && API.IsHudPreferenceSwitchedOn() && !API.IsPlayerSwitchInProgress() && API.IsScreenFadedIn() && !API.IsPauseMenuActive() && !API.IsFrontendFading() && !API.IsPauseMenuRestarting());
        }

        private void SetVoiceEnabled(bool isEnabled, bool net = true)
        {
            enabled = isEnabled;
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

            if (net) Client.TriggerServerEvent("Voice.PlayerVoiceLevelChanged", voiceLevel);

            script.Log($"Voice level set to {voiceLevelStr}");
        }

        private void SetVoiceChannel(int voiceChannel, bool net = true)
        {
            if (voiceChannel < -1) voiceChannel = -1;
            channel = voiceChannel;

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

        private void DrawLevel(int r, int g, int b, int a)
        {
            API.SetTextFont(4);
            API.SetTextScale(0.5f, 0.5f);
            API.SetTextColour(r, g, b, a);
            API.SetTextDropshadow(0, 0, 0, 0, 255);
            API.SetTextDropShadow();
            API.SetTextOutline();
            API.BeginTextCommandDisplayText("STRING");
            API.AddTextComponentSubstringPlayerName((channel < 0) ? voiceLevelStr : $"Channel: {channel}");
            API.EndTextCommandDisplayText(0.175f, 0.92f);
        }

        private void DrawTextOnScreen(string text, float xPosition, float yPosition, float size, Alignment justification, int font, bool disableTextOutline)
        {
            API.SetTextFont(font);
            API.SetTextScale(1.0f, size);
            if (justification == Alignment.Right)
            {
                API.SetTextWrap(0f, xPosition);
            }
            API.SetTextJustification((int)justification);
            if (!disableTextOutline) { API.SetTextOutline(); }
            API.BeginTextCommandDisplayText("STRING");
            API.AddTextComponentSubstringPlayerName(text);
            API.EndTextCommandDisplayText(xPosition, yPosition);
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
