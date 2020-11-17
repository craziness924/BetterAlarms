using PulsarPluginLoader; //

namespace Alarms
{
    public class Plugin : PulsarPlugin
    {
        public override string Version => "2.1";

        public override string Author => "craziness924";

        public override string Name => "Alarms";

        public override string HarmonyIdentifier()
        {
            return "craziness924.Alarms";
        }
    }
}
