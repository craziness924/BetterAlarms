using PulsarModLoader; 

namespace Alarms
{
    public class Mod : PulsarMod
    {
        public override string Version => "2.35";

        public override string Author => "craziness924";

        public override string Name => "Alarms";

        public override string HarmonyIdentifier()
        {
            return "craziness924.Alarms";
        }
    }
}
