using VinaFrameworkClient.Core;
using FiveM_Voice.Modules;

namespace FiveM_Voice
{
    public class Client : BaseClient
    {
        public Client()
        {
            AddModule(typeof(NuiModule));
            AddModule(typeof(VoiceModule));
        }
    }
}
