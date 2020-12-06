using VinaFrameworkServer.Core;
using Vina_VoiceServer.Modules;

namespace Vina_VoiceServer
{
    public class Server : BaseServer
    {
        public Server()
        {
            AddModule(typeof(VoiceModule));
        }
    }
}
