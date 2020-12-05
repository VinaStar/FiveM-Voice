using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CitizenFX.Core.Native;
using VinaFrameworkClient.Core;

namespace FiveM_Voice.Modules
{
    public class NuiModule : Module
    {
        public NuiModule(Client client) : base(client)
        {

        }

        #region BASE EVENTS



        #endregion
        #region MODULE EVENTS



        #endregion
        #region MODULE METHODS

        private void SendActionData(string action, dynamic data)
        {
            API.SendNuiMessage("{\"action\": \"" + action + "\", \"data\": \"" + data + "\"}");
        }

        public void SetEnabled(bool enabled)
        {
            SendActionData("SetEnabled", enabled);
        }

        public void SetVisibility(bool visible)
        {
            SendActionData("SetVisibility", visible);
        }

        public void SetVoiceLevel(string level)
        {
            SendActionData("SetVoiceLevel", level);
        }

        public void SetChannel(int channel)
        {
            SendActionData("SetChannel", channel);
        }

        public void UpdateTalking(string talkings)
        {
            SendActionData("UpdateTalking", talkings);
        }

        #endregion
    }
}
