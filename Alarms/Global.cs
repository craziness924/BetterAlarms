using HarmonyLib;
using PulsarModLoader.Utilities;

namespace Alarms
{
    class Global //defaults woah, make sure to change the reset command values as well!
    {
        public static float hull = .4f;
        public static float shield = .98f;
        public static int firecount = 15;
        public static float o2 = .15f;
        public static int flashcount = 10;
        public static float horspeed = 300f;
        public static float verspeed = 0f;
       
    }
    [HarmonyPatch(typeof(PLServer), "Start")]
    class LoadPatch
    {
        static void Postfix()
        {
            string[] settings = PLXMLOptionsIO.Instance.CurrentOptions.GetStringValue("AlarmSettings").Split(' ');
            if (settings.Length > 3)
            {
                bool errors = true;
                errors &= float.TryParse(settings[0], out Global.hull);
                errors &= float.TryParse(settings[1], out Global.shield);
                errors &= float.TryParse(settings[2], out Global.o2);
                errors &= int.TryParse(settings[3], out Global.firecount);
                errors &= int.TryParse(settings[4], out Global.flashcount);
                errors &= float.TryParse(settings[5], out Global.horspeed);
                errors &= float.TryParse(settings[6], out Global.verspeed);
                if (!errors)
                 { 
                Logger.Info("Something went wrong loading Alarm settings, could be conflicting mod. " + settings.Join());
                 } 
            }
            Logger.Info("Failed to load Alarms settings, or settings left on default!");
        }


    }
}
