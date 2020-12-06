using VinaFrameworkClient.Core;
using Vina_VoiceClient.Modules;

namespace Vina_VoiceClient
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
