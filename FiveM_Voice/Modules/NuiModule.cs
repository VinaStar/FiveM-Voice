using VinaFrameworkClient.Core;

namespace FiveM_Voice.Modules
{
    public class NuiModule : Module
    {
        public NuiModule(Client client) : base(client)
        {

        }

        #region MODULE METHODS

        public void SetEnabled(bool enabled)
        {
            Client.SendNuiActionData("SetEnabled", enabled);
        }

        public void SetVisibility(bool visible)
        {
            Client.SendNuiActionData("SetVisibility", visible);
        }

        public void SetVoiceLevel(string level)
        {
            Client.SendNuiActionData("SetVoiceLevel", level);
        }

        public void SetChannel(int channel)
        {
            Client.SendNuiActionData("SetChannel", channel);
        }

        public void UpdateTalking(string talkings)
        {
            Client.SendNuiActionData("UpdateTalking", talkings);
        }

        #endregion
    }
}
