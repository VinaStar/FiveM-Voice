using VinaFrameworkServer.Core;
using FiveM_VoiceServer.Modules;

namespace FiveM_VoiceServer
{
    public class Server : BaseServer
    {
        public Server()
        {
            AddModule(typeof(VoiceModule));
        }
    }
}
