using HarmonyLib;
using PulsarPluginLoader.Utilities;

namespace Alarms
{
    class Global //defaults woah, make sure to change the reset command values as well!
    {
        public static float hull = .4f;
        public static float shield = .98f;
        public static int firecount = 15;
        public static float o2 = .15f; 
       
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
                if (!errors)
                 { 
                Logger.Info("Something went wrong loading Alarm settings, could be conflicting mod. " + settings.Join());
                 } 
            }
            Logger.Info("Failed to load Alarms settings, or settings left on default!");
        }


    }
}
